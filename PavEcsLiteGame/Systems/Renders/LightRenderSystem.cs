using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    partial class LightRenderSystem : IEcsRunSystem, IEcsSystemSpec
    {

        private interface ILightSource
        {
            ref readonly PositionComponent Pos();
            ref readonly LightSourceComponent LightSource();
            ref AreaResultComponent<float> Result();
        }

        [Entity]
        private partial struct LightToRenderDynamicEnt : ILightSource
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly LightSourceComponent LightSource();
            public partial ref readonly SpeedComponent Speed();
            public partial ref AreaResultComponent<float> Result();
        }

        [Entity]
        private partial struct LightToRenderStaticEnt : ILightSource
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly LightSourceComponent LightSource();
            public partial ExcludeComponent<SpeedComponent> Speed();
            public partial ref AreaResultComponent<float> Result();
        }

        private int _staticLightVersion = -1;

        [Entity(SkipFilter = true)]
        private partial struct LightLayerEnt
        {
            public partial ref AreaResultComponent<LightValueComponent> Light();
        }


        private readonly MapData<LightValueComponent> _lightMap;
        private readonly MapData<LightValueComponent> _lightMapStatic;

        [Entity]
        private partial struct MapLoadedEnt
        {
            public partial ref readonly MapLoadedEvent Loaded();
        }

        private readonly LightValueComponent _ambient = default;

        public LightRenderSystem()
        {
            _lightMap = new MapData<LightValueComponent>();
            _lightMapStatic = new MapData<LightValueComponent>();

            _ambient = new LightValueComponent()
            {
                Value = 1
            };
        }


        public void Run(EcsSystems systems)
        {
            foreach (var ent in _providers.MapLoadedEntProvider)
            {
                var size = ent.Loaded().Size;
                _lightMap.Init(size);
                 _lightMapStatic.Init(size);
            }

            int currentVersion = 0;

            foreach (var ent in _providers.LightToRenderStaticEntProvider)
            {
                var rev = ent.Result().Revision;
                currentVersion ^= (ent.GetRawId() << 8 | rev);
            }

            if (currentVersion != _staticLightVersion)
            {
                _staticLightVersion = currentVersion;
                _lightMapStatic.Fill(_ambient);

                CalcualteStaticLight(_lightMapStatic);
                Debug.Print("Re-render static light");
            }


            _lightMap.Clear();
            _lightMap.CopyFrom(_lightMapStatic);
            //CalculateLightMap(_lightToRenderDynamicSpec.Filter, _lightMap);
            CalcualteDynamicLight(_lightMap);


            _providers.LightLayerEntProvider.New().Light().Data = _lightMap;
           
        }

        private void CalcualteDynamicLight(MapData<LightValueComponent> lightMap)
        {
            foreach (var ent in _providers.LightToRenderDynamicEntProvider)
            {
                CalculateLightMap(ent, lightMap);
            }
        }
        private void CalcualteStaticLight(MapData<LightValueComponent> lightMap)
        {
            foreach(var ent in _providers.LightToRenderStaticEntProvider)
            {
                CalculateLightMap(ent, lightMap);
            }
        }
        private void CalculateLightMap<T>(T ent, MapData<LightValueComponent> lightMap)
            where T : struct, ILightSource
        {

            //int radiusSq = (lightData.Radius + 1) * (lightData.Radius + 1);
            //float invRadiusSq = 1.0f / radiusSq;
            var context = new LightDataContext(in ent.LightSource(), ent.Pos());
            IMapData<PositionComponent, LightValueComponent> m = lightMap;
            m.Merge(ent.Result().Data, context, _lightMergeDelegate);
        }
        //private void CalculateLightMap(EcsFilter ecsFilter, MapData<LightValueComponent> lightMap)
        //{
        //    var (posPool, lightDataPool, _) = _lightToRenderDynamicSpec.IncludeReadonly;
        //    var lightResultPool = _lightToRenderDynamicSpec.Include.Pool1;

        //    foreach (EcsUnsafeEntity ent in ecsFilter)
        //    {
        //        ref readonly var lightData = ref lightDataPool.Get(ent);
        //        ref readonly var center = ref posPool.Get(ent);
        //        ref var lightResult = ref lightResultPool.Get(ent);

        //        //int radiusSq = (lightData.Radius + 1) * (lightData.Radius + 1);
        //        //float invRadiusSq = 1.0f / radiusSq;
        //        var context = new LightDataContext(in lightData, center);
        //        IMapData<PositionComponent, LightValueComponent> m = lightMap; 
        //        m.Merge(lightResult.Data, context, _lightMergeDelegate);
        //    }
        //}

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

            public LightDataContext(in LightSourceComponent lightData, PositionComponent center)
            {
                Center = center;
                BasicParameters = lightData.BasicParameters;
                RadiusSq = (lightData.Radius + 1) * (lightData.Radius + 1);
                InvRadiusSq = 1.0f / RadiusSq;
            }

        }
    }

}
