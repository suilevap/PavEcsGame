using System;
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

        private static RenderItem[] _lights = new[]
        {
            new RenderItem(_lightShade[0], ConsoleColor.DarkGray),
            new RenderItem(_lightShade[0], ConsoleColor.Gray),
            new RenderItem(_lightShade[0], ConsoleColor.White),

            new RenderItem(_lightShade[1], ConsoleColor.DarkGray),
            new RenderItem(_lightShade[1], ConsoleColor.Gray),
            new RenderItem(_lightShade[1], ConsoleColor.White),

            new RenderItem(_lightShade[2], ConsoleColor.DarkGray),
            new RenderItem(_lightShade[2], ConsoleColor.Gray),
            new RenderItem(_lightShade[2], ConsoleColor.White),
        };

        private static ConsoleColor[] _lightColors = new[]
        {
            ConsoleColor.DarkYellow,
            ConsoleColor.Yellow,
            ConsoleColor.White
        };

        private static float[] _dith = new[] {0.75f,0.8f, 1f, 1.1f, 1.25f};

        private Random _rnd = new Random(41);

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
                :this(new SymbolComponent(value){MainColor = color})
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
        private readonly MapData<float> _lightMap;
        private readonly EcsFilterSpec<EcsSpec<MapLoadedEvent>, EcsSpec, EcsSpec> _mapLoadedSpec;
        private readonly EcsFilterSpec<EcsSpec<PositionComponent, SymbolComponent>, EcsSpec<MarkAsRenderedTag>, EcsSpec> _itemsToRenderSpec;
        private readonly EcsFilterSpec<EcsSpec<FieldOfViewResultComponent, LightSourceComponent>, EcsSpec, EcsSpec> _lightToRenderSpec;
        private readonly EcsEntityFactorySpec<EcsSpec<RenderItemCommand>> _renderCommandFactory;

        public PrepareForRenderSystem(EcsUniverse universe, MapData<EcsPackedEntityWithWorld> map)
        {
            _map = map;
            _lightMap = new MapData<float>();
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
                    EcsSpec<FieldOfViewResultComponent, LightSourceComponent>.Build())
                .End();

            _renderCommandFactory = universe
                .CreateEntityFactorySpec(
                    EcsSpec<RenderItemCommand>.Build());


        }

        public void Run(EcsSystems systems)
        {
            InitBuffers();

            UpdateLightMap();


            Helper.Swap(ref _bufferCurrentFrame, ref _bufferPreviousFrame);
            ForwardRender();

            ApplyLightMap(_lightMap);

            CreateRenderCommands();

            void InitBuffers()
            {
                foreach (var ent in _mapLoadedSpec.Filter)
                {
                    var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
                    _bufferCurrentFrame.Init(size);
                    _bufferPreviousFrame.Init(size);
                    _lightMap.Init(size);
                }
            }


            void UpdateLightMap()
            {
                _lightMap.Clear();
                var lightDataPool = _lightToRenderSpec.Include.Pool1;
                foreach (var ent in _lightToRenderSpec.Filter)
                {
                    ref var lightData = ref lightDataPool.Get(ent);
                    int radiusSq = lightData.Radius * lightData.Radius;
                    float invRadiusSq = 1.0f / radiusSq;
                    var center = lightData.Center;
                    _lightMap.Merge(lightData.Data, LightMerge);

                    float LightMerge(Int2 pos, float sourceValue, float targetValue)
                    {
                        var sqD = pos.DistanceSquare(center);
                        if (sqD <= radiusSq)
                        {
                            var lightValue = targetValue * (1 - sqD * invRadiusSq);
                            sourceValue += lightValue;
                        }

                        return sourceValue;
                    }
                }
            }

            void ApplyLightMap(IMapData<PositionComponent, float> mapData)
            {

                foreach (var ( pos, lightValue) in mapData.GetAll())
                {
                    ref var renderItem = ref _bufferCurrentFrame.GetRef(pos);
                    //get previous state
                    if (lightValue <= 0.1f)
                    {
                        renderItem = _bufferPreviousFrame.GetRef(pos);
                    }

                    //var dith = _dith[(pos.Value.X + pos.Value.Y) % _dith.Length];
                    renderItem = Light(ref renderItem, lightValue);

                }

                RenderItem Light(ref RenderItem item, in float lightValue)
                {
                    if (lightValue <= 0.1f)
                    {
                        //Symbol = SymbolComponent.Empty;
                        item.Symbol.MainColor = ConsoleColor.DarkGray;
                    }
                    else
                    {

                        var lightColor = GetLightColor(lightValue); 
                        if (item.Symbol.Value == SymbolComponent.Empty.Value ||
                            item.Symbol.Value == default)
                        {
                            item.Symbol.Value = '.';
                            item.Symbol.MainColor = lightColor; //ConsoleColor.Gray;
                            //return _lights[(int) (Math.Clamp(lightValue, 0, 1) * (_lights.Length-1))];
                        }
                        else
                        {

                            item.Symbol.MainColor = lightColor;
                        }
                    }

                    return item;
                }

                ConsoleColor GetLightColor(float lightValue)
                {
                    var lightColor = _lightColors
                        [(int)(Math.Clamp(lightValue, 0, 0.999) * (_lightColors.Length))];
                    return lightColor;
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
            }
        }
    }
}
