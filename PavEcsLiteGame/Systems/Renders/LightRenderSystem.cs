using System;
using System.Diagnostics;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsGame.Utils;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    internal partial class LightRenderSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private LightValueComponent _ambient;
        private Color _lastAmbientColor;
        private int _mapRevision;

        private readonly MapData<LightValueComponent> _lightMap;

        private readonly MapData<LightValueComponent> _lightMapStatic;
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

        private readonly MergeDelegate<LightDataContext, PositionComponent, LightValueComponent, float>
            _lightMergeDelegate = LightMerge;

        private int _staticLightVersion = -1;
        private int _lastMapRevision = -1;

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

        [Entity]
        private partial struct LightLayerEnt
        {
            public partial ref AreaResultComponent<LightValueComponent> Light();
        }

        [Entity]
        private partial struct MapLoadedEnt
        {
            public partial ref readonly MapLoadedEvent Loaded();
        }

        [Entity]
        private partial struct AmbientLightEnt
        {
            public partial ref readonly AmbientLightComponent Ambient();
        }

        private readonly struct LightDataContext
        {
            public readonly PositionComponent Center;
            public readonly LightValueComponent BasicParameters;
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

        public LightRenderSystem()
        {
            _lightMap = new MapData<LightValueComponent>();
            _lightMapStatic = new MapData<LightValueComponent>();
        }


        public void Run(IEcsSystems systems)
        {
            foreach (var ent in _providers.MapLoadedEntProvider)
            {
                var size = ent.Loaded().Size;
                _lightMap.Init(size);
                _lightMapStatic.Init(size);
                _mapRevision++;
            }

            // Sum all ambient light contributions; fallback to default dark gray if none
            var ambientColor = new Color(0, 0, 0, 0);
            var hasAmbient = false;
            foreach (var ent in _providers.AmbientLightEntProvider)
            {
                var c = ent.Ambient().Color;
                ambientColor = new Color(ambientColor.R + c.R, ambientColor.G + c.G, ambientColor.B + c.B, 0);
                hasAmbient = true;
            }
            if (!hasAmbient)
                ambientColor = new Color(64, 64, 64, 0); // default: known-but-dark tiles still visible
            _ambient = new LightValueComponent { AccumulatedColor = ambientColor };

            var currentVersion = _mapRevision;

            foreach (var ent in _providers.LightToRenderStaticEntProvider)
            {
                var rev = ent.Result().Revision;
                currentVersion ^= (ent.GetRawId() << 8) | rev;
            }

            if (ambientColor != _lastAmbientColor || _mapRevision != _lastMapRevision)
            {
                _lastAmbientColor = ambientColor;
                _lastMapRevision = _mapRevision;
                _staticLightVersion = (currentVersion + 1) % 256;
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

            if (_providers.LightLayerEntProvider.Filter.IsEmpty())
                _providers.LightLayerEntProvider.New().Light().Data = _lightMap;
        }

        private void CalcualteDynamicLight(MapData<LightValueComponent> lightMap)
        {
            foreach (var ent in _providers.LightToRenderDynamicEntProvider) CalculateLightMap(ent, lightMap);
        }

        private void CalcualteStaticLight(MapData<LightValueComponent> lightMap)
        {
            foreach (var ent in _providers.LightToRenderStaticEntProvider) CalculateLightMap(ent, lightMap);
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

        private static void LightMerge(in LightDataContext c, in PositionComponent pos,
            ref LightValueComponent sourceValue, in float targetValue)
        {
            var sqD = pos.Value.DistanceSquare(c.Center);
            if (sqD > c.RadiusSq) return;

            var attenuated = (byte)(targetValue * (1f - sqD * c.InvRadiusSq) * c.BasicParameters.AccumulatedColor.R);
            if (attenuated == 0) return;

            var type = (LightType)c.BasicParameters.AccumulatedColor.A;
            var gradColor = GetGradientForType(type).GetByRateLerp(attenuated);
            var cur = sourceValue.AccumulatedColor;

            // Add RGB (Color(int,int,int,int) constructor clamps per channel automatically)
            // OR the type flag into A — do NOT add A (Color.operator+ would corrupt the bitmask)
            sourceValue.AccumulatedColor = new Color(
                cur.R + gradColor.R,
                cur.G + gradColor.G,
                cur.B + gradColor.B,
                cur.A | (byte)type);
        }

        private static Color[] GetGradientForType(LightType type) => type switch
        {
            LightType.Fire => LightGradients.FireGradient,
            LightType.Electricity => LightGradients.ElectricityGradient,
            LightType.Acid => LightGradients.AcidGradient,
            _ => LightGradients.NoneGradient,
        };
    }
}
