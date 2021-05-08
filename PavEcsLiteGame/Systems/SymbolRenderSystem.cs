using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;
using PavEcsGame.Extensions;
using System.Threading;
using Leopotam.EcsLite;

namespace PavEcsGame.Systems
{
    public class SymbolRenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntity> _map;

        private readonly EcsFilter/*<PreviousPositionComponent>*/ _clearPreviousPosFilter;
        private readonly EcsPool<PreviousPositionComponent> _prevPosPool;
        private readonly EcsFilter/*<PositionComponent, SymbolComponent>.Exclude<MarkAsRenderedTag>*/ _updateCurrentPosFilter;
        private readonly EcsPool<PositionComponent> _posPool;
        private readonly EcsPool<SymbolComponent> _symbolPool;
        private readonly EcsPool<MarkAsRenderedTag> _markAsRenderPool;

        private readonly EcsWorld _world;


        public SymbolRenderSystem(IReadOnlyMapData<PositionComponent, EcsPackedEntity> map, EcsWorld world)
        {
            _map = map;
            _world = world;
            _clearPreviousPosFilter = _world.Filter<PreviousPositionComponent>().End();
            _prevPosPool = _world.GetPool<PreviousPositionComponent>();

            _updateCurrentPosFilter =
                _world.Filter<PositionComponent>().Inc<SymbolComponent>().Exc<MarkAsRenderedTag>().End();

            _posPool = _world.GetPool<PositionComponent>();
            _symbolPool = _world.GetPool<SymbolComponent>();
            _markAsRenderPool = _world.GetPool<MarkAsRenderedTag>();
        }
        public void Init(EcsSystems systems)
        {
            Console.CursorVisible = false;
        }

        public void Run(EcsSystems systems)
        {
            //Console.Clear();
            bool smthChanged = !_clearPreviousPosFilter.IsEmpty() || !_updateCurrentPosFilter.IsEmpty();
            foreach(var i in _clearPreviousPosFilter)
            {
                ref var prevPos = ref _prevPosPool.Get(i);
                if (!_map.Get(prevPos.Value).Unpack(_world, out var entity))
                {
                    RenderItem(in prevPos.Value, in SymbolComponent.Empty);
                    smthChanged = true;
                }
                _markAsRenderPool.Del(entity);
            }

            foreach(var i in _updateCurrentPosFilter)
            {
                ref var pos = ref _posPool.Get(i);
                ref var symbol = ref _symbolPool.Get(i);
                RenderItem(in pos, in symbol);
                _markAsRenderPool.Del(i);
            }

            // if (smthChanged)
            // {
            //     Thread.Sleep(50);
            // }

            static void RenderItem(in PositionComponent pos, in SymbolComponent symbol)
            {
                Console.SetCursorPosition(pos.Value.X, pos.Value.Y);
                Console.Write(symbol.Value);
            }
        }
    }
}
