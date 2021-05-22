using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PaveEcsGame;
using PaveEcsGame.Area;

namespace PavEcsGame.Systems
{
    class FieldOfViewSystem : IEcsRunSystem
    {
        private struct FieldOfViewCalculated : ITag {}

        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        private readonly EcsFilterSpec<
            EcsSpec<PositionComponent, LightSourceComponent>, 
            EcsSpec<AreaResultComponent<float>>, 
            EcsSpec<FieldOfViewCalculated>> _lightSourceSpec;

        private readonly EcsFilterSpec<
            EcsSpec<FieldOfViewCalculated, PositionComponent, PreviousPositionComponent, LightSourceComponent, AreaResultComponent<float>>,
            EcsSpec,
            EcsSpec> _lightSourceOutdated;

        private readonly FieldOfViewComputationInt2 _fieldOfView;
        //private readonly EcsFilterSpec<EcsSpec<MapLoadedEvent>, EcsSpec, EcsSpec> _mapLoadedSpec;
        private readonly Func<Int2, Int2, bool> _hasObstacles;
        private (Int2 delta, float value)[] _fieldOfViewResult;

        public FieldOfViewSystem(
            EcsUniverse universe,
            IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> map)
        {
            _map = map;
            universe
                .Build(ref _lightSourceSpec)
                .Build(ref _lightSourceOutdated);

            _fieldOfView = new FieldOfViewComputationInt2();
            _hasObstacles = HasObstacle;
        }
        public void Run(EcsSystems systems)
        {
            //foreach (var ent in _mapLoadedSpec.Filter)
            //{
            //    var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
            //    _lightMap.Init(size);
            //}

            //_lightMap.Clear();
            var fieldOfViewCalculatedPool = _lightSourceOutdated.Include.Pool1;
            foreach (var ent in _lightSourceOutdated.Filter)
            {
                fieldOfViewCalculatedPool.Del(ent);
            }

            //add new one
            var posPool = _lightSourceSpec.Include.Pool1;
            var lightDataPool = _lightSourceSpec.Include.Pool2;
            var resultPool = _lightSourceSpec.Optional.Pool1;
            foreach (EcsUnsafeEntity ent in _lightSourceSpec.Filter)
            {
                var pos = posPool.Get(ent);
                var lightData = lightDataPool.Get(ent);
                var radius = lightData.Radius;

                UpdateFieldOfViewData(ent, pos, radius);
                
                fieldOfViewCalculatedPool.Add(ent);
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
                //foreach (var (d, value) in lights)
                for (int i = 0; i < count; i++)
                {
                    ref var item = ref _fieldOfViewResult[i]; 
                    var p = pos.Value + item.delta;
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

        private bool HasObstacle(Int2 pos, Int2 delta)
        {
            if (_map.Get(_map.GetSafePos(pos + delta)).Unpack(out _, out EcsUnsafeEntity rawEnt))
            {
                //TODO: check if it solid
                return true;
            }
            return false;
        }
    }
}
