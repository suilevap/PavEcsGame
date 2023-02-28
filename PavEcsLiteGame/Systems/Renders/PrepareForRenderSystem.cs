using System;
using System.Diagnostics;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using PavEcsGame.Utils;

namespace PavEcsGame.Systems.Renders
{
    public class PrepareForRenderSystem : IEcsRunSystem, IEcsSystemSpec
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
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<MapLoadedEvent>> _mapLoadedSpec;
        private readonly EcsFilterSpec<
            EcsReadonlySpec<PositionComponent, SymbolComponent>,
            EcsReadonlySpec<MarkAsRenderedTag, SpeedComponent>, 
            EcsSpec> _itemsToRenderSpec;
        private readonly EcsFilterSpec<
            EcsSpec<AreaResultComponent<LightValueComponent>>,
            EcsSpec, 
            EcsSpec> _lightToRenderSpec;


        private readonly EcsFilterSpec<
            EcsReadonlySpec<AreaResultComponent<VisibilityType>>,
            EcsSpec,
            EcsSpec> _playerFieldOfViewSpec;

        private readonly EcsEntityFactorySpec<EcsSpec<RenderItemCommand>> _renderCommandFactory;

        public PrepareForRenderSystem(EcsUniverse universe, MapData<EcsPackedEntityWithWorld> map)
        {
            _map = map;
            _bufferCurrentFrame = new MapData<RenderItem>();
            _bufferPreviousFrame = new MapData<RenderItem>();

            universe
                .Register(this)
                .Build(ref _mapLoadedSpec)
                .Build(ref _itemsToRenderSpec)
                .Build(ref _lightToRenderSpec)
                .Build(ref _playerFieldOfViewSpec)
                .Build(ref _renderCommandFactory);
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
                foreach (EcsUnsafeEntity ent in _mapLoadedSpec.Filter)
                {
                    var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
                    _bufferCurrentFrame.Init(size);
                    _bufferPreviousFrame.Init(size);
                }
            }


            void PostEffects(IReadOnlyMapData<PositionComponent, VisibilityType> visibilityMap)
            {
                var lightDataPool = _lightToRenderSpec.Include.Pool1;
                Debug.Assert(_lightToRenderSpec.Filter.GetEntitiesCount() <= 1, "Only one light map is supported");

                foreach (var ent in _lightToRenderSpec.Filter)
                {
                    ref var lightData = ref lightDataPool.Get(ent);
                    ApplyPostEffect(lightData.Data, visibilityMap);
                    //handle and delete this
                    lightDataPool.Del(ent);
                }
            }

            IReadOnlyMapData<PositionComponent,VisibilityType>? TryGetVisibilityMap()
            {
                var visibilityDataPool = _playerFieldOfViewSpec.Include.Pool1;
                Debug.Assert(_playerFieldOfViewSpec.Filter.GetEntitiesCount() <= 1, "Only one visibility map is supported");
                foreach (EcsUnsafeEntity ent in _playerFieldOfViewSpec.Filter)
                {
                    ref readonly var visibilityComponent = ref visibilityDataPool.Get(ent);
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

                var (posPool, symbolPool) = _itemsToRenderSpec.Include;
                var speedPool = _itemsToRenderSpec.Optional.Pool2;
                foreach (EcsUnsafeEntity ent in _itemsToRenderSpec.Filter)
                {
                    ref readonly var pos = ref posPool.Get(ent);
                    if (!visibilityMap.IsValid(pos))
                        continue;
                    var visibility = visibilityMap.Get(pos);
                    if (visibility.HasFlag(VisibilityType.Visible)
                       || (!speedPool.Has(ent) && visibility.HasFlag(VisibilityType.Known))) 
                    {
                        ref readonly var symbol = ref symbolPool.Get(ent);

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
