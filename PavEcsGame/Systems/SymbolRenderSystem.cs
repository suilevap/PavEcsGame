using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;
using PavEcsGame.Extensions;

namespace PavEcsGame.Systems
{
    partial class SymbolRenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        private IReadOnlyMapData<PositionComponent, EcsEntity> _map;

        private EcsFilter<PreviousPositionComponent> _clearPreviousPosFilter;

        private EcsFilter<PositionComponent, SymbolComponent>.Exclude<MarkAsRenderedTag> _updateCurrentPosFilter;

        public void Init()
        {
            Console.CursorVisible = false;
        }

        public void Run()
        {
            //Console.Clear();
            foreach(var i in _clearPreviousPosFilter)
            {
                ref var prevPos = ref _clearPreviousPosFilter.Get1(i);
                if (!_map.Get(prevPos.Value).IsAlive())
                {
                    RenderItem(in prevPos.Value, in SymbolComponent.Empty);
                }
                _clearPreviousPosFilter.GetEntity(i).Del<MarkAsRenderedTag>();
            }

            foreach(var i in _updateCurrentPosFilter)
            {
                ref var pos = ref _updateCurrentPosFilter.Get1(i);
                ref var symbol = ref _updateCurrentPosFilter.Get2(i);
                RenderItem(in pos, in symbol);
                _updateCurrentPosFilter.GetEntity(i).Tag<MarkAsRenderedTag>();
            }

            static void RenderItem(in PositionComponent pos, in SymbolComponent symbol)
            {
                Console.SetCursorPosition(pos.Value.X, pos.Value.Y);
                Console.Write(symbol.Value);
            }
        }
    }
}
