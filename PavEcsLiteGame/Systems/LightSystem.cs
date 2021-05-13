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
    class LightSystem : IEcsRunSystem
    {
        private readonly IReadOnlyMapData<Int2, EcsPackedEntityWithWorld> _map;
        private readonly IMapData<Int2, float> _lightMap;

        private readonly EcsFilterSpec<EcsSpec<PositionComponent, LightSourceComponent>, EcsSpec, EcsSpec> _lightSourceSpec;
        private readonly FieldOfViewComputationInt2 _fieldOfView;
        private readonly EcsFilterSpec<EcsSpec<MapLoadedEvent>, EcsSpec, EcsSpec> _mapLoadedSpec;

        public LightSystem(
            EcsUniverse universe,
            IReadOnlyMapData<Int2, EcsPackedEntityWithWorld> map,
            IMapData<Int2, float> lightMap)
        {
            _map = map;
            _lightMap = lightMap;


            _lightSourceSpec = universe
                .StartFilterSpec(
                    EcsSpec<PositionComponent, LightSourceComponent>.Build())
                .End();

            _mapLoadedSpec = universe
                .StartFilterSpec(
                    EcsSpec<MapLoadedEvent>.Build())
                .End();

            _fieldOfView = new FieldOfViewComputationInt2();
        }
        public void Run(EcsSystems systems)
        {
            foreach (var ent in _mapLoadedSpec.Filter)
            {
                var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
                _lightMap.Init(size);
            }

            _lightMap.Clear();

            //add new one
            var posPool = _lightSourceSpec.Include.Pool1;
            var lightDataPool = _lightSourceSpec.Include.Pool2;
            foreach (var ent in _lightSourceSpec.Filter)
            {
                var pos = posPool.Get(ent);
                var lightData = lightDataPool.Get(ent);

                var radiusSq = lightData.Radius * lightData.Radius;
                var lights = _fieldOfView.Compute(pos.Value, lightData.Radius, HasObstacle);
                foreach (var (p, value) in lights)
                {
                    var sqD = pos.Value.DistanceSquare(in p);
                    if (sqD <= radiusSq)
                    {
                        var lightValue = value * (1 - sqD / radiusSq);
                        ref var v = ref _lightMap.GetRef(_lightMap.GetSafePos(in p));
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
