using System;
using System.Collections.Generic;
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
        private readonly EcsFilterSpec<EcsSpec<PositionComponent, LightSourceComponent, AreaResultComponent<float>>, EcsSpec, EcsSpec> _lightToRenderSpec;
        private readonly EcsEntityFactorySpec<EcsSpec<AreaResultComponent<LightValueComponent>>> _lightLayerFactory;

        private readonly MapData<LightValueComponent> _lightMap;
        private readonly EcsFilterSpec<EcsSpec<MapLoadedEvent>, EcsSpec, EcsSpec> _mapLoadedSpec;

        public LightRenderSystem(EcsUniverse universe)
        {
            _lightMap = new MapData<LightValueComponent>();
            _lightToRenderSpec = universe
                .StartFilterSpec(
                    EcsSpec<PositionComponent, LightSourceComponent, AreaResultComponent<float>>.Build())
                .End();

            _lightLayerFactory = universe.CreateEntityFactorySpec(
                EcsSpec<AreaResultComponent<LightValueComponent>>.Build());

            _mapLoadedSpec = universe
                .StartFilterSpec(
                    EcsSpec<MapLoadedEvent>.Build())
                .End();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var ent in _mapLoadedSpec.Filter)
            {
                var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
                _lightMap.Init(size);
            }

            _lightMap.Clear();
            var (posPool, lightDataPool, lightResultPool) = _lightToRenderSpec.Include;
            foreach (var ent in _lightToRenderSpec.Filter)
            {
                var lightData = lightDataPool.Get(ent);
                int radiusSq = (lightData.Radius + 1) * (lightData.Radius + 1);
                float invRadiusSq = 1.0f / radiusSq;
                var center = posPool.Get(ent);

                ref var lightResult = ref lightResultPool.Get(ent);

                _lightMap.Merge(lightResult.Data, LightMerge);

                void LightMerge(in PositionComponent pos, ref LightValueComponent sourceValue, in float targetValue)
                {
                    var sqD = pos.Value.DistanceSquare(center);
                    if (sqD <= radiusSq)
                    {
                        var lightValue = (byte)(targetValue * (1 - sqD * invRadiusSq) * lightData.BasicParameters.Value);
                        //if (lightValue > sourceValue.Value)
                        //{
                        //    sourceValue.LightType |= lightData.BasicParameters.LightType;
                        //}
                        if (sourceValue.LightType.HasFlag(lightData.BasicParameters.LightType))
                        {
                            sourceValue.Value = Math.Min((byte) (sourceValue.Value + lightValue), (byte) 255);
                        }
                        else if (lightValue > sourceValue.Value)
                        {
                            sourceValue.Value = lightValue;
                            sourceValue.LightType = lightData.BasicParameters.LightType;
                        }
                        //var lightValue = targetValue * (1 - sqD * invRadiusSq);
                        //Color c;
                        //if (lightValue > 0.75f)
                        //{
                        //    c = Color.Lerp(lightColor, Color.One, (lightValue - 0.75f) * 4);
                        //}
                        //else
                        //{
                        //    c = lightColor * ((lightValue - 0.25f)/0.75f);
                        //}
                        //sourceValue = sourceValue +  c;
                    }
                }
            }

            _lightLayerFactory.NewUnsafeEntity()
                .Add(_lightLayerFactory.Pools,
                    new AreaResultComponent<LightValueComponent>()
                    {
                        Data = _lightMap
                    });
        }

        //public void AddColor(ref Color target,in Color baseColor, float lightValue)
        //{
        //    target.R  = (byte)baseColor.R * lightValue;
        //}
    }

}
