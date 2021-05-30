using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PaveEcsGame;

namespace PavEcsGame.Systems.Renders
{
    class LightRenderSystem : IEcsRunSystem
    {
        private readonly EcsFilterSpec<
            EcsSpec<PositionComponent, LightSourceComponent, AreaResultComponent<float>, SpeedComponent>, 
            EcsSpec,
            EcsSpec> _lightToRenderDynamicSpec;

        private readonly EcsFilterSpec<
            EcsSpec<PositionComponent, LightSourceComponent, AreaResultComponent<float>>,
            EcsSpec,
            EcsSpec<SpeedComponent>> _lightToRenderStaticSpec;

        private int _staticLightVersion = 0;

        private readonly EcsEntityFactorySpec<EcsSpec<AreaResultComponent<LightValueComponent>>> _lightLayerFactory;

        private readonly MapData<LightValueComponent> _lightMap;
        private readonly MapData<LightValueComponent> _lightMapStatic;
        private readonly EcsFilterSpec<EcsSpec<MapLoadedEvent>, EcsSpec, EcsSpec> _mapLoadedSpec;

        private readonly LightValueComponent _ambient = default;

        public LightRenderSystem(EcsUniverse universe)
        {
            _lightMap = new MapData<LightValueComponent>();
            _lightMapStatic = new MapData<LightValueComponent>();
            universe
                .Build(ref _lightToRenderDynamicSpec)
                .Build(ref _lightToRenderStaticSpec)
                .Build(ref _lightLayerFactory)
                .Build(ref _mapLoadedSpec);

            _ambient = new LightValueComponent()
            {
                Value = 1
            };
        }

        public void Run(EcsSystems systems)
        {
            foreach (var ent in _mapLoadedSpec.Filter)
            {
                var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
                _lightMap.Init(size);
                _lightMapStatic.Init(size);
            }


            var (posPool, lightDataPool, lightResultPool, _) = _lightToRenderDynamicSpec.Include;


            int currentVersion = 0;
            foreach (var ent in _lightToRenderStaticSpec.Filter)
            {
                var rev = lightResultPool.Get(ent).Revision;
                currentVersion ^= (ent<<8 | ent);
            }

            if (currentVersion != _staticLightVersion)
            {
                _staticLightVersion = currentVersion;
                _lightMapStatic.Fill(_ambient);

                CalculateLightMap(_lightToRenderStaticSpec.Filter, _lightMapStatic);
                Debug.Print("Re-render static light");
            }

            _lightMap.Clear();
            _lightMap.CopyFrom(_lightMapStatic);
            CalculateLightMap(_lightToRenderDynamicSpec.Filter, _lightMap);

            _lightLayerFactory.NewUnsafeEntity()
                .Add(_lightLayerFactory.Pools,
                    new AreaResultComponent<LightValueComponent>()
                    {
                        Data = _lightMap
                    });
           
        }
        private void CalculateLightMap(EcsFilter ecsFilter, MapData<LightValueComponent> lightMap)
        {
            var (posPool, lightDataPool, lightResultPool, _) = _lightToRenderDynamicSpec.Include;

            foreach (var ent in ecsFilter)
            {
                var lightData = lightDataPool.Get(ent);
                var center = posPool.Get(ent);
                ref var lightResult = ref lightResultPool.Get(ent);

                //int radiusSq = (lightData.Radius + 1) * (lightData.Radius + 1);
                //float invRadiusSq = 1.0f / radiusSq;
                var context = new LightDataContext(ref lightData, center);
                IMapData<PositionComponent, LightValueComponent> m = lightMap; 
                m.Merge(lightResult.Data, context, _lightMergeDelegate);
            }
        }

        private  readonly MergeDelegate<LightDataContext, PositionComponent, LightValueComponent, float>
            _lightMergeDelegate = LightMerge;
        private static void LightMerge(in LightDataContext c, in PositionComponent pos, ref LightValueComponent sourceValue, in float targetValue)
        {
            var sqD = pos.Value.DistanceSquare(c.Center);
            if (sqD <= c.RadiusSq)
            {
                var lightValue = (byte)(targetValue * (1 - sqD * c.InvRadiusSq) * c.BasicParameters.Value);

                if (sourceValue.LightType.HasFlag(c.BasicParameters.LightType))
                {
                    sourceValue.Value = (byte)Math.Min((sourceValue.Value + lightValue), 255);
                }
                else if (lightValue > sourceValue.Value)
                {
                    sourceValue.Value = lightValue;
                    sourceValue.LightType = c.BasicParameters.LightType;
                }
            }
        }

        private readonly struct LightDataContext
        {
            public readonly PositionComponent Center;
            public readonly  LightValueComponent BasicParameters;
            public readonly int RadiusSq;
            public readonly float InvRadiusSq;

            public LightDataContext(ref LightSourceComponent lightData, PositionComponent center)
            {
                Center = center;
                BasicParameters = lightData.BasicParameters;
                RadiusSq = (lightData.Radius + 1) * (lightData.Radius + 1);
                InvRadiusSq = 1.0f / RadiusSq;
            }

        }
    }

}
