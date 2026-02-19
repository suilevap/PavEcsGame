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
    public partial class PrepareForRenderSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        private readonly Providers _providers;

        private MapData<RenderItem> _bufferCurrentFrame;
        private MapData<RenderItem> _bufferPreviousFrame;

        private struct RenderItem : IEquatable<RenderItem>
        {
            public SymbolComponent Symbol;
            public readonly Color BackgroundColor;

            private RenderItem(SymbolComponent symbol, Color back = default)
            {
                Symbol = symbol;
                BackgroundColor = back == default ? Color.Zero : back;
            }

            internal RenderItem(char value, Color color)
                : this(new SymbolComponent(value) { MainColor = color })
            {
            }

            public void Merge(in SymbolComponent symbol)
            {
                if (Symbol.Depth <= symbol.Depth) Symbol = symbol;
            }

            public static bool operator ==(RenderItem a, RenderItem b)
            {
                return a.Symbol.Value == b.Symbol.Value &&
                       a.Symbol.MainColor == b.Symbol.MainColor &&
                       a.BackgroundColor == b.BackgroundColor;
            }

            public static bool operator !=(RenderItem a, RenderItem b)
            {
                return !(a == b);
            }

            public override string ToString()
            {
                return Symbol.ToString();
            }

            public bool Equals(RenderItem other)
            {
                return this == other;
            }

            public override bool Equals(object? obj)
            {
                return obj is RenderItem other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Symbol, BackgroundColor.GetHashCode());
            }
        }

        [Entity]
        private partial struct MapLoadedEnt
        {
            public partial ref readonly MapLoadedEvent Loaded();
        }

        [Entity]
        private partial struct ItemToRenderEnt
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly SymbolComponent Symbol();

            public partial OptionalComponent<MarkAsRenderedTag> MarkAsRendered();
            public partial OptionalComponent<SpeedComponent> Speed();
        }

        [Entity]
        private partial struct LightToRenderEnt
        {
            public partial RequiredComponent<AreaResultComponent<LightValueComponent>> LightData();
        }

        [Entity]
        private partial struct FovEnt
        {
            public partial ref AreaResultComponent<VisibilityType> Data();
        }


        [Entity(SkipFilter = true)]
        private partial struct RenderCommandEnt
        {
            public partial ref RenderItemCommand RenderCommand();
        }

        [Entity]
        private readonly partial struct WindowResizeEnt
        {
            public partial ref readonly WindowResizeEvent Resize();
        }


        public PrepareForRenderSystem(EcsSystems universe, MapData<EcsPackedEntityWithWorld> map)
            : this(universe)
        {
            _map = map;
            _bufferCurrentFrame = new MapData<RenderItem>();
            _bufferPreviousFrame = new MapData<RenderItem>();
        }

        public void Run(IEcsSystems systems)
        {
            InitBuffers();

            var visibilityMap = TryGetVisibilityMap();
            if (visibilityMap == null)
                return;
            Helper.Swap(ref _bufferCurrentFrame, ref _bufferPreviousFrame);

            ForwardRender(visibilityMap);

            PostEffects(visibilityMap);

            CreateRenderCommands();

            void InitBuffers()
            {
                foreach (var ent in _providers.MapLoadedEntProvider)
                {
                    var size = ent.Loaded().Size;
                    _bufferCurrentFrame.Init(size);
                    _bufferPreviousFrame.Init(size);
                }

                foreach (var ent in _providers.WindowResizeEntProvider)
                {
                    _bufferCurrentFrame.Clear();
                }
            }


            void PostEffects(IReadOnlyMapData<PositionComponent, VisibilityType> visibilityMap)
            {
                var count = _providers.LightToRenderEntProvider.Filter.GetEntitiesCount();
                //Debug.Assert(count <= 1, "Only one light map is supported");
                foreach (var ent in _providers.LightToRenderEntProvider)
                {
                    ref var lightData = ref ent.LightData().Get();
                    ApplyPostEffect(lightData.Data, visibilityMap);
                    //handle and delete this
                    ent.LightData().Remove();
                }
            }

            IReadOnlyMapData<PositionComponent, VisibilityType>? TryGetVisibilityMap()
            {
                Debug.Assert(_providers.FovEntProvider.Filter.GetEntitiesCount() <= 1,
                    "Only one visibility map is supported");
                foreach (var ent in _providers.FovEntProvider)
                {
                    ref readonly var visibilityComponent = ref ent.Data();
                    return visibilityComponent.Data;
                }

                return null;
            }

            void ApplyPostEffect(IMapData<PositionComponent, LightValueComponent> lightMap,
                IReadOnlyMapData<PositionComponent, VisibilityType> visibilityMap)
            {
                foreach (var (pos, lightValue) in lightMap.GetAll())
                {
                    ref var renderItem = ref _bufferCurrentFrame.GetRef(pos);
                    var visibility = visibilityMap.Get(pos);
                    var light = lightValue;

                    if ((visibility & VisibilityType.Known) != 0)
                    {
                        renderItem = Light(ref renderItem, ref light, visibility, in pos);
                    }
                    else
                    {
                        if (visibilityMap.CheckNeighbours(false, pos, _bufferCurrentFrame,
                                (result, p, value, buffer) =>
                                    result || (value & VisibilityType.Known) != 0 &&
                                           buffer.GetRef(p).Symbol.Depth == Depth.Back))
                            renderItem = new RenderItem('?', new Color(139, 0, 0)); // DarkRed
                    }
                }

                RenderItem Light(ref RenderItem item, ref LightValueComponent light, VisibilityType visibility,
                    in PositionComponent pos)
                {
                    var ligthValue = light.Value;
                    var lightColor = ToRgbColor(light);

                    if (item.Symbol.IsEmpty)
                    {
                        if ((visibility & VisibilityType.Visible) != 0 || pos.Value.IsHexPos())
                        {
                            item.Symbol.Value = '.';
                            item.Symbol.MainColor = lightColor;
                        }
                    }
                    else
                    {
                        item.Symbol.MainColor = lightColor;
                    }

                    return item;
                }
            }

            void ForwardRender(IReadOnlyMapData<PositionComponent, VisibilityType> visibilityMap)
            {
                _bufferCurrentFrame.Clear();


                foreach (var ent in _providers.ItemToRenderEntProvider)
                {
                    ref readonly var pos = ref ent.Pos();
                    if (!visibilityMap.IsValid(pos))
                        continue;
                    var visibility = visibilityMap.Get(pos);
                    if ((visibility & VisibilityType.Visible) != 0
                        || (!ent.Speed().Has() && (visibility & VisibilityType.Known) != 0))
                    {
                        ref readonly var symbol = ref ent.Symbol();

                        ref var renderItem = ref _bufferCurrentFrame.GetRef(pos);
                        renderItem.Merge(symbol);
                    }
                }
            }

            void CreateRenderCommands()
            {
                IMapData<PositionComponent, RenderItem> currentBuffer = _bufferCurrentFrame;
                var redraws = 0;
                foreach (var (pos, renderItem) in currentBuffer.GetAll())
                {
                    ref var prevRenderItem = ref _bufferPreviousFrame.GetRef(in pos);
                    if (prevRenderItem != renderItem)
                    {
                        _providers.RenderCommandEntProvider
                            .New()
                            .RenderCommand() = new RenderItemCommand
                        {
                            Symbol = renderItem.Symbol,
                            BackgroundColor = renderItem.BackgroundColor,
                            Position = pos
                        };
                        redraws++;
                    }
                }

                if (redraws > 0) Debug.Print("Render commands: {0}", redraws);
            }
        }


        /// <summary>
        /// Converts light value to 24-bit RGB color using smooth gradient interpolation.
        /// Supports additive RGB blending for overlapping light types (e.g., Fire + Electricity).
        /// </summary>
        private static Color ToRgbColor(LightValueComponent lightValue)
        {
            if (lightValue.Value == 0)
                return Color.Zero;

            Color result = Color.Zero;
            byte intensity = lightValue.Value;

            // Additive RGB blending for proper light mixing
            if ((lightValue.LightType & LightType.Fire) != 0)
                result = result + LightGradients.FireGradient.GetByRateLerp(intensity);
            if ((lightValue.LightType & LightType.Electricity) != 0)
                result = result + LightGradients.ElectricityGradient.GetByRateLerp(intensity);
            if ((lightValue.LightType & LightType.Acid) != 0)
                result = result + LightGradients.AcidGradient.GetByRateLerp(intensity);

            if (lightValue.LightType == LightType.None)
                result = LightGradients.NoneGradient.GetByRateLerp(intensity);

            return result; // Clamped by Color.operator+
        }
    }
}
