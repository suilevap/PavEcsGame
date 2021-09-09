using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using PavEcsGame.Area;

namespace PavEcsGame.Systems
{
    class FieldOfViewSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private struct FieldOfViewCalculated 
        {
            public PositionComponent Position;
            public FieldOfViewRequestEvent Request;
        }

        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        //private readonly EcsFilterSpec<
        //    EcsReadonlySpec<PositionComponent, FieldOfViewRequestEvent>, 
        //    EcsSpec<AreaResultComponent<float>, FieldOfViewCalculated>, 
        //    EcsSpec> _filedOfViewSourcesSpec;
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<PositionComponent>, EcsSpec<FieldOfViewRequestEvent>>
            .Opt<EcsSpec<AreaResultComponent<float>, FieldOfViewCalculated>> _filedOfViewSourcesSpec;

        private readonly EcsEntityFactorySpec<
            EcsReadonlySpec<PositionComponent, SpeedComponent>> _obstacleSpec;

        private readonly FieldOfViewComputationInt2 _fieldOfView;
        private readonly Func<Int2, Int2, bool> _hasObstacles;
        private (Int2 delta, float value)[]? _fieldOfViewResult;

        public FieldOfViewSystem(
            EcsUniverse universe,
            IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> map)
        {
            _map = map;
            universe
                .Register(this)
                .Build(ref _filedOfViewSourcesSpec)
                .Build(ref _obstacleSpec);

            _fieldOfView = new FieldOfViewComputationInt2();
            _hasObstacles = HasObstacle;
        }

        public void Run(EcsSystems systems)
        {

            var posPool = _filedOfViewSourcesSpec.IncludeReadonly.Pool1;
            var requestPool = _filedOfViewSourcesSpec.Include.Pool1;

            var (resultPool, previousInputPool) = _filedOfViewSourcesSpec.Optional;
            foreach (EcsUnsafeEntity ent in _filedOfViewSourcesSpec.Filter)
            {
                ref readonly var pos = ref posPool.Get(ent);
                ref var request = ref requestPool.Get(ent);

                ref var prevInput = ref previousInputPool.Ensure(ent, out var isNew);

                if (isNew || prevInput.Position != pos || prevInput.Request != request)
                {
                    var radius = request.Radius;
                    prevInput.Position = pos;
                    prevInput.Request = request;
                    
                    UpdateFieldOfViewData(ent, pos, radius);
                }

                requestPool.Del(ent);
            }

            void UpdateFieldOfViewData(EcsUnsafeEntity ent, PositionComponent pos, int radius)
            {
                ref var result = ref resultPool.Ensure(ent, out var isNew);
                if (isNew)
                {
                    result.Data = new MapData<float>();
                    result.Data.Init(_map.MaxPos - _map.MinPos);
                }
                else
                {
                    result.Data.Clear();
                    result.Revision++;
                }

                var radiusSq = radius * radius;
                _fieldOfView.Compute(pos.Value, radius, _hasObstacles, ref _fieldOfViewResult, out var count);
                for (int i = 0; i < count; i++)
                {
                    ref var item = ref _fieldOfViewResult[i]; 
                    var p = pos.Value + item.delta;
                    if (_map.IsValid(p))
                    {
                        var sqD = pos.Value.DistanceSquare(in p);
                        if (sqD <= radiusSq)
                        {
                            var lightValue = item.value;// * (1 - sqD / radiusSq);
                            ref var v = ref result.Data.GetRef(result.Data.GetSafePos(new PositionComponent(p)));
                            v += lightValue;
                        }
                    }
                }
            }
        }

        private bool HasObstacle(Int2 pos, Int2 delta)
        {
            var p = pos + delta;
            if (!_map.IsValid(p))
                return true;
            if (_map.Get(p).Unpack(out _, out EcsUnsafeEntity rawEnt))
            {
                //TODO: check if it solid
                return !_obstacleSpec.Pools.Pool2.Has(rawEnt);
            }
            return false;
        }
    }
}
