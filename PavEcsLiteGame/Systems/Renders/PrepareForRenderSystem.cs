using System;
using System.Diagnostics;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using PavEcsGame.Utils;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    public partial class PrepareForRenderSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private const string _lightShade = "░▒▓";

        private struct RenderItem : IEquatable<RenderItem>
        {

            public SymbolComponent Symbol;
            public ConsoleColor BackgroundColor;

            private RenderItem(SymbolComponent symbol, ConsoleColor back = ConsoleColor.Black)
            {
                Symbol = symbol;
                BackgroundColor = back;
            }

            internal RenderItem(char value, ConsoleColor color)
                : this(new SymbolComponent(value) { MainColor = color })
            {

            }

            public void Merge(in SymbolComponent symbol)
            {
                if (Symbol.Depth <= symbol.Depth)
                {
                    Symbol = symbol;
                }
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
            
            public bool Equals(RenderItem other) => this == other;

            public override bool Equals(object? obj) => obj is RenderItem other && Equals(other);

            public override int GetHashCode()
            {
                return HashCode.Combine(Symbol, (int)BackgroundColor);
            }

        }

        private MapData<RenderItem> _bufferCurrentFrame;
        private MapData<RenderItem> _bufferPreviousFrame;

        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        private Providers _providers;

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

        private readonly EcsEntityFactorySpec<EcsSpec<RenderItemCommand>> _renderCommandFactory;

        [Entity(SkipFilter = true)]
        private partial struct RenderCommandEnt
        {
            public partial ref RenderItemCommand RenderCommand();
        }


        public PrepareForRenderSystem(EcsSystems universe, MapData<EcsPackedEntityWithWorld> map)
            :this(universe)
        {
            _map = map;
            _bufferCurrentFrame = new MapData<RenderItem>();
            _bufferPreviousFrame = new MapData<RenderItem>();
        }

        public void Run(EcsSystems systems)
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
            }


            void PostEffects(IReadOnlyMapData<PositionComponent, VisibilityType> visibilityMap)
            {

                Debug.Assert(_providers.LightToRenderEntProvider.Filter.GetEntitiesCount() <= 1, "Only one light map is supported");

                foreach (var ent in _providers.LightToRenderEntProvider)
                {
                    ref var lightData = ref ent.LightData().Get();
                    ApplyPostEffect(lightData.Data, visibilityMap);
                    //handle and delete this
                    ent.LightData().Remove();
                }
            }

            IReadOnlyMapData<PositionComponent,VisibilityType>? TryGetVisibilityMap()
            {
                Debug.Assert(_providers.FovEntProvider.Filter.GetEntitiesCount() <= 1, "Only one visibility map is supported");
                foreach (var ent in _providers.FovEntProvider)
                {
                    ref readonly var visibilityComponent = ref ent.Data();
                    return visibilityComponent.Data;
                }
                return null;
            }

            void ApplyPostEffect(IMapData<PositionComponent, LightValueComponent> lightMap, IReadOnlyMapData<PositionComponent, VisibilityType> visibilityMap)
            {

                foreach (var ( pos,  lightValue) in lightMap.GetAll())
                {
                    ref var renderItem = ref _bufferCurrentFrame.GetRef(pos);
                    var visibility = visibilityMap.Get(pos);
                    var light = lightValue;

                    if (visibility.HasFlag(VisibilityType.Known))
                    {
                        renderItem = Light(ref renderItem, ref light, visibility, in pos);
                    }
                    else
                    {
                        if (visibilityMap.CheckNeighbours(
                            false,
                            pos,
                            (r, p, v) => r || (v.HasFlag(VisibilityType.Known) && _bufferCurrentFrame.GetRef(p).Symbol.Depth == Depth.Back)))
                        {
                            renderItem = new RenderItem('?', ConsoleColor.DarkRed);
                        }
                    }

                }

                RenderItem Light(ref RenderItem item, ref LightValueComponent light, VisibilityType visibility, in PositionComponent pos)
                {
                    byte ligthValue = light.Value;
                    var lightColor = ToConsoleColor(light);

                    if (item.Symbol.IsEmpty)
                    {
                        if (visibility.HasFlag(VisibilityType.Visible) || pos.Value.IsHexPos())
                        {
                            item.Symbol.Value = '.';
                            item.Symbol.MainColor = lightColor;
                        }
                    }
                    else
                    {
                        item.Symbol.MainColor = lightColor;
                    }

                    //item.BackgroundColor = visibility.HasFlag(VisibilityType.Visible) 
                    //    ? ConsoleColor.Blue 
                    //    : ConsoleColor.DarkBlue;

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
                    if (visibility.HasFlag(VisibilityType.Visible)
                       || (!ent.Speed().Has() && visibility.HasFlag(VisibilityType.Known)))
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
                int redraws = 0;
                foreach (var (pos, renderItem) in currentBuffer.GetAll())
                {
                    ref var prevRenderItem = ref _bufferPreviousFrame.GetRef(in pos);
                    if (prevRenderItem != (renderItem))
                    {
                        _providers.RenderCommandEntProvider
                            .New()
                            .RenderCommand() = new RenderItemCommand()
                            {
                                Symbol = renderItem.Symbol,
                                BackgroundColor = renderItem.BackgroundColor,
                                Position = pos,
                            };
                        redraws++;
                    }
                }

                if (redraws > 0)
                {
                    Debug.Print("Render commands: {0}", redraws);
                }
            }
        }


        //index Brgb
        // 0   0000  dark black
        // 1   0001  dark blue
        // 2   0010  dark green
        // 3   0011  dark cyan
        // 4   0100  dark red
        // 5   0101  dark purple
        // 6   0110  dark yellow(brown)
        // 7   0111  dark white(light grey)
        // 8   1000  bright black(dark grey)
        // 9   1001  bright blue
        //10   1010  bright green
        //11   1011  bright cyan    
        //12   1100  bright red
        //13   1101  bright purple
        //14   1110  bright yellow
        //15   1111  bright white

        private static readonly ConsoleColor[] _fireColors = new[]
        {
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkYellow,
            ConsoleColor.Yellow,
            ConsoleColor.Yellow,
            ConsoleColor.Yellow,
            ConsoleColor.Yellow,
            ConsoleColor.Yellow,
            ConsoleColor.Yellow,
            ConsoleColor.Yellow,
            ConsoleColor.Yellow,
            ConsoleColor.White
        };

        private static readonly ConsoleColor[] _electroColors = new[]
        {
            ConsoleColor.DarkBlue,
            ConsoleColor.DarkCyan,
            ConsoleColor.Blue,
            ConsoleColor.Cyan,
        };

        private static readonly ConsoleColor[] _acidColors = new[]
        {
            ConsoleColor.DarkGreen,
            ConsoleColor.Green,
        };

        private static readonly ConsoleColor[] _noneColors = new[]
        {
            ConsoleColor.DarkGray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.Gray,
            ConsoleColor.White,
        };

        private static readonly ConsoleColor[] _brightColors = new[]
        {
            ConsoleColor.Black,
            ConsoleColor.DarkGray,
            //ConsoleColor.Gray,
            //ConsoleColor.White,
        };


        public ConsoleColor ToConsoleColor(LightValueComponent lightValue)
        {
            if (lightValue.Value == 0)
                return ConsoleColor.Black;

            int color = 0;
            if (lightValue.LightType.HasFlag(LightType.Fire))
            {
                color |= (int)_fireColors.GetByRate(lightValue.Value);
            }
            if (lightValue.LightType.HasFlag(LightType.Electricity))
            {
                color |= (int)_electroColors.GetByRate(lightValue.Value);
            }
            if (lightValue.LightType.HasFlag(LightType.Acid))
            {
                color |= (int)_acidColors.GetByRate(lightValue.Value);
            }

            if (lightValue.LightType == LightType.None)
            {
                color = (int)_noneColors.GetByRate(lightValue.Value);
            }

            return (ConsoleColor)color;
        }

        public static System.ConsoleColor FromColor(System.Drawing.Color c)
        {
            int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0; // Bright bit
            index |= (c.R > 64) ? 4 : 0; // Red bit
            index |= (c.G > 64) ? 2 : 0; // Green bit
            index |= (c.B > 64) ? 1 : 0; // Blue bit
            return (System.ConsoleColor)index;
        }
    }
}
