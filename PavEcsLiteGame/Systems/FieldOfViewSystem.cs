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
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        private readonly EcsFilterSpec<EcsSpec<PositionComponent, LightSourceComponent>, EcsSpec<AreaResultComponent<float>>, EcsSpec> _lightSourceSpec;
        private readonly FieldOfViewComputationInt2 _fieldOfView;
        //private readonly EcsFilterSpec<EcsSpec<MapLoadedEvent>, EcsSpec, EcsSpec> _mapLoadedSpec;

        public FieldOfViewSystem(
            EcsUniverse universe,
            IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> map)
        {
            _map = map;


            _lightSourceSpec = universe
                .StartFilterSpec(
                    EcsSpec<PositionComponent, LightSourceComponent>.Build())
                .Optional(
                    EcsSpec<AreaResultComponent<float>>.Build())
                .End();

            //_mapLoadedSpec = universe
            //    .StartFilterSpec(
            //        EcsSpec<MapLoadedEvent>.Build())
            //    .End();

            _fieldOfView = new FieldOfViewComputationInt2();
        }
        public void Run(EcsSystems systems)
        {
            //foreach (var ent in _mapLoadedSpec.Filter)
            //{
            //    var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
            //    _lightMap.Init(size);
            //}

            //_lightMap.Clear();

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
            }

            void UpdateFieldOfViewData(EcsUnsafeEntity ent, PositionComponent pos, int radius)
            {
                ref var result = ref resultPool.GetOrAdd(ent, out var exists);
                if (!exists)
                {
                    result.Data = new MapData<float>();
                    result.Data.Init(_map.MaxPos - _map.MinPos);
                }
                else
                {
                    result.Data.Clear();
                }

                var radiusSq = radius * radius;
                var lights = _fieldOfView.Compute(pos.Value, radius, HasObstacle);
                foreach (var (p, value) in lights)
                {
                    var sqD = pos.Value.DistanceSquare(in p);
                    if (sqD <= radiusSq)
                    {
                        var lightValue = value;// * (1 - sqD / radiusSq);
                        ref var v = ref result.Data.GetRef(result.Data.GetSafePos(new PositionComponent(p)));
                        v += lightValue;
                    }
                }
            }
        }

        private bool HasObstacle(Int2 pos)
        {
            if (_map.Get(_map.GetSafePos(pos)).Unpack(out _, out EcsUnsafeEntity rawEnt))
            {
                //TODO: check if it solid
                return true;
            }
            return false;
        }
    }
}
