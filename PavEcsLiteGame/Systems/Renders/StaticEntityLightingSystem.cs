using System;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    internal partial class StaticEntityLightingSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        private struct StaticLightCacheComponent
        {
            public LightValueComponent LastBaseLight;  // light at entity's pos when last computed
            public LightValueComponent CachedOutputLight; // averaged neighbor light (cached result)
        }

        [Entity]
        private partial struct StaticEntityEnt
        {
            public partial ref readonly PositionComponent Pos();         // include filter
            public partial ExcludeComponent<SpeedComponent> Speed();     // exclude moving entities
            public partial OptionalComponent<StaticLightCacheComponent> Cache(); // per-entity cache
        }

        [Entity]
        private partial struct LightLayerEnt
        {
            public partial ref AreaResultComponent<LightValueComponent> Light(); // the live light map
        }

        public StaticEntityLightingSystem(EcsSystems universe,
            IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> map)
            : this(universe)
        {
            _map = map;
        }

        public void Run(IEcsSystems systems)
        {
            IMapData<PositionComponent, LightValueComponent> lightMap = null;
            foreach (var ent in _providers.LightLayerEntProvider)
            {
                lightMap = ent.Light().Data;
                break;
            }
            if (lightMap == null) return;
            foreach (var entity in _providers.StaticEntityEntProvider)
            {
                var pos = entity.Pos();
                var currentBaseLight = lightMap.Get(pos);
                ref var cache = ref entity.Cache().Ensure(out var isNew);

                LightValueComponent outputLight;
                if (!isNew && cache.LastBaseLight.AccumulatedColor == currentBaseLight.AccumulatedColor)
                {
                    outputLight = cache.CachedOutputLight; // cache hit
                }
                else
                {
                    outputLight = ComputeResultLightValue(pos.Value, lightMap); // cache miss or first use
                    cache.LastBaseLight = currentBaseLight;
                    cache.CachedOutputLight = outputLight;
                }

                lightMap.GetRef(pos) = outputLight;
            }
        }

        private static readonly Int2[] NeighbourOffsets = {
            new Int2(-1,-1), new Int2(0,-1), new Int2(1,-1),
            new Int2(-1, 0),                 new Int2(1, 0),
            new Int2(-1, 1), new Int2(0, 1), new Int2(1, 1)
        };

        private LightValueComponent ComputeResultLightValue(Int2 pos,
            IMapData<PositionComponent, LightValueComponent> lightMap)
        {
            var best = lightMap.Get(pos);
            foreach (var d in NeighbourOffsets)
            {
                var npos = new PositionComponent(pos + d);
                if (!_map.IsValid(npos)) continue;
                if (_map.Get(npos).Unpack(out _, out EcsUnsafeEntity _)) continue; // cell occupied — skip

                var n = lightMap.Get(npos);
                var b = best.AccumulatedColor;
                best.AccumulatedColor = new Color(
                    Math.Max(b.R, n.AccumulatedColor.R),
                    Math.Max(b.G, n.AccumulatedColor.G),
                    Math.Max(b.B, n.AccumulatedColor.B),
                    (byte)(best.LightTypes | n.LightTypes));  // union via named accessors
            }
            return best;
        }
    }
}
