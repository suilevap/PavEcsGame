using System;
using PavEcsGame.Components;
using Leopotam.EcsLite;
using Microsoft.VisualBasic.CompilerServices;
using PavEcsGame.Components.Events;
using PavEcsGame.Components.SystemComponents;
using PavEcsSpec.EcsLite;
using PaveEcsGame.Utils;

namespace PavEcsGame.Systems
{
    public class SymbolBufferRenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        private struct RenderItem
        {
            public SymbolComponent Symbol;
            public ConsoleColor BackgroundColor;

            public RenderItem(SymbolComponent symbol, ConsoleColor back = ConsoleColor.Black)
            {
                Symbol = symbol;
                BackgroundColor = back;
            }

            public void Merge(in SymbolComponent symbol)
            {
                if (Symbol.Depth <= symbol.Depth)
                {
                    Symbol = symbol;
                }
            }


            public void Light(in float lightValue)
            {
                if (lightValue < 0.01f)
                {
                    //Symbol = SymbolComponent.Empty;
                    Symbol.MainColor = ConsoleColor.DarkGray;
                }
                else
                {
                    if (Symbol.Value == SymbolComponent.Empty.Value ||
                        Symbol.Value == default )
                    {
                        Symbol.Value = '.';
                        Symbol.MainColor = ConsoleColor.DarkGray;
                    }
                    else
                    {
                    }
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

        public SymbolBufferRenderSystem(EcsUniverse universe, MapData<EcsPackedEntityWithWorld> map, MapData<float> lightMap)
        {
            _map = map;
            _lightMap = lightMap;
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

        }

        public void Init(EcsSystems systems)
        {
            Console.CursorVisible = false;
        }

        public void Run(EcsSystems systems)
        {
            foreach (var ent in _mapLoadedSpec.Filter)
            {
                var size = _mapLoadedSpec.Include.Pool1.Get(ent).Size;
                _bufferCurrentFrame.Init(size);
                _bufferPreviousFrame.Init(size);
            }

            Helper.Swap(ref _bufferCurrentFrame, ref _bufferPreviousFrame);
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

            IMapData<PositionComponent, float> lightData = _lightMap;
            foreach (var ( pos, lightValue)  in lightData.GetAll())
            {
                ref var renderItem = ref _bufferCurrentFrame.GetRef(pos);
                renderItem.Light(lightValue);
            }


            IMapData<PositionComponent, RenderItem> currentBuffer = _bufferCurrentFrame;
            int redraws = 0;
            foreach (var (pos, renderItem) in currentBuffer.GetAll())
            {
                ref var prevRenderItem = ref _bufferPreviousFrame.GetRef(in pos);
                if (prevRenderItem != (renderItem))
                {
                    RenderItem(in pos, in renderItem);
                    redraws++;
                }
            }

            static void RenderItem(in PositionComponent pos, in RenderItem item)
            {
                Console.SetCursorPosition(pos.Value.X, pos.Value.Y);
                if (item != default)
                {
                    Console.ForegroundColor = item.Symbol.MainColor;
                    Console.BackgroundColor = item.BackgroundColor;
                    Console.Write(item.Symbol.Value);
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(SymbolComponent.Empty.Value);
                }
            }

        }
    }
}
