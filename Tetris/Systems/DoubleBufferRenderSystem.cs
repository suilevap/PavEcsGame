using PavEcsGame.Components;
using PavEcsGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PavEcsSpec.EcsLite;
using Leopotam.EcsLite;
using PavEcsGame.Utils;
using System.Diagnostics;
using Leopotam.Ecs.Types;

namespace Tetris.Systems
{
    internal class DoubleBufferRenderSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly Int2 _screenSize;

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

        private MapData<RenderItem> _bufferCurrentFrame = new MapData<RenderItem>();
        private MapData<RenderItem> _bufferPreviousFrame = new MapData<RenderItem>();

        private readonly EcsEntityFactorySpec<EcsSpec<RenderItemCommand>> _renderCommandFactory;

        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<PositionComponent, SymbolComponent>> _itemsToRenderSpec;

        public DoubleBufferRenderSystem(EcsUniverse universe, Int2 screenSize)
        {
            _screenSize = screenSize;
            _bufferCurrentFrame.Init(screenSize);
            _bufferPreviousFrame.Init(screenSize);
            universe
                .Register(this)
                .Build(ref _itemsToRenderSpec)
                .Build(ref _renderCommandFactory);
        }

        public void Run(IEcsSystems systems)
        {
            Helper.Swap(ref _bufferCurrentFrame, ref _bufferPreviousFrame);

            ForwardRender();

            CreateRenderCommands();

            void ForwardRender()
            {
                _bufferCurrentFrame.Clear();

                var (posPool, symbolPool) = _itemsToRenderSpec.Include;
                foreach (EcsUnsafeEntity ent in _itemsToRenderSpec.Filter)
                {
                    ref readonly var pos = ref posPool.Get(ent);
                    if (pos.Value.X < 0 ||
                        pos.Value.Y < 0 ||
                        pos.Value.X >= _screenSize.X ||
                        pos.Value.Y >= _screenSize.Y)
                        continue;
                    ref readonly var symbol = ref symbolPool.Get(ent);

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
    }
}
