using System;
using System.Diagnostics;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PaveEcsGame;
using PaveEcsGame.Utils;

namespace PavEcsGame.Systems.Renders
{
    public class PrepareForRenderSystem : IEcsRunSystem
    {
        private const string _lightShade = "░▒▓";

        private struct RenderItem
        {
            public SymbolComponent Symbol;
            public ConsoleColor BackgroundColor;

            public RenderItem(SymbolComponent symbol, ConsoleColor back = ConsoleColor.Black)
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

        }


        private MapData<RenderItem> _bufferCurrentFrame;
        private MapData<RenderItem> _bufferPreviousFrame;

        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;
        private readonly EcsFilterSpec<EcsSpec<MapLoadedEvent>, EcsSpec, EcsSpec> _mapLoadedSpec;
        private readonly EcsFilterSpec<EcsSpec<PositionComponent, SymbolComponent>, EcsSpec<MarkAsRenderedTag>, EcsSpec> _itemsToRenderSpec;
        private readonly EcsFilterSpec<EcsSpec<AreaResultComponent<LightValueComponent>>, EcsSpec, EcsSpec> _lightToRenderSpec;
        private readonly EcsEntityFactorySpec<EcsSpec<RenderItemCommand>> _renderCommandFactory;

        public PrepareForRenderSystem(EcsUniverse universe, MapData<EcsPackedEntityWithWorld> map)
        {
            _map = map;
            _bufferCurrentFrame = new MapData<RenderItem>();
            _bufferPreviousFrame = new MapData<RenderItem>();

            _mapLoadedSpec = universe
                .StartFilterSpec(
                    EcsSpec<MapLoadedEvent>.Build())
                .End();
            _itemsToRenderSpec = universe
                .StartFilterSpec(
                    EcsSpec<PositionComponent, SymbolComponent>.Build())
                .Optional(
                    EcsSpec<MarkAsRenderedTag>.Build())
                .End();
            _lightToRenderSpec = universe
                .StartFilterSpec(
                    EcsSpec<AreaResultComponent<LightValueComponent>>.Build())
                .End();

            _renderCommandFactory = universe
                .CreateEntityFactorySpec(
                    EcsSpec<RenderItemCommand>.Build());


        }

        public void Run(EcsSystems systems)
        {
            InitBuffers();

            Helper.Swap(ref _bufferCurrentFrame, ref _bufferPreviousFrame);
            ForwardRender();

            ApplyLight();

            CreateRenderCommands();

            void InitBuffers()
            {
                foreach (var ent in _mapLoadedSpec.Filter)
                {
                    var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
                    _bufferCurrentFrame.Init(size);
                    _bufferPreviousFrame.Init(size);
                }
            }


            void ApplyLight()
            {
                var lightDataPool = _lightToRenderSpec.Include.Pool1;
                foreach (var ent in _lightToRenderSpec.Filter)
                {
                    var lightData = lightDataPool.Get(ent);
                    ApplyLightMap(lightData.Data);
                    //handle and delete this
                    lightDataPool.Del(ent);
                }
            }

            void ApplyLightMap(IMapData<PositionComponent, LightValueComponent> mapData)
            {

                foreach (var (pos, lightValue) in mapData.GetAll())
                {
                    ref var renderItem = ref _bufferCurrentFrame.GetRef(pos);
                    var lightColor = ToConsoleColor(lightValue);
                    //get previous state
                    if (lightColor == ConsoleColor.Black)
                    {
                        renderItem = _bufferPreviousFrame.GetRef(pos);
                    }

                    renderItem = Light(ref renderItem, lightColor);

                }

                RenderItem Light(ref RenderItem item, in ConsoleColor lightColor)
                {
                    if (lightColor == ConsoleColor.Black)
                    {
                        item.Symbol.MainColor = ConsoleColor.DarkGray;
                    }
                    else
                    {

                        if (item.Symbol.Value == SymbolComponent.Empty.Value ||
                            item.Symbol.Value == default)
                        {
                            item.Symbol.Value = '.';
                            item.Symbol.MainColor = lightColor;
                        }
                        else
                        {

                            item.Symbol.MainColor = lightColor;
                        }
                    }

                    return item;
                }
            }

            void ForwardRender()
            {
                _bufferCurrentFrame.Clear();

                var posPool = _itemsToRenderSpec.Include.Pool1;
                var symbolPool = _itemsToRenderSpec.Include.Pool2;
                foreach (var ent in _itemsToRenderSpec.Filter)
                {
                    ref var pos = ref posPool.Get(ent);
                    ref var symbol = ref symbolPool.Get(ent);

                    ref var renderItem = ref _bufferCurrentFrame.GetRef(pos);
                    renderItem.Merge(symbol);
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
                        _renderCommandFactory.NewUnsafeEntity()
                            .Add(_renderCommandFactory.Pools,

                                new RenderItemCommand()
                                {
                                    Symbol = renderItem.Symbol,
                                    BackgroundColor = renderItem.BackgroundColor,
                                    Position = pos,
                                });
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
