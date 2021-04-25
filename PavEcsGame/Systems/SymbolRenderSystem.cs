using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;
using PavEcsGame.LeoEcsExtensions;

namespace PavEcsGame.Systems
{
    partial class SymbolRenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        private IMapData<PositionComponent, EcsEntity> _map;
        private EcsFilter<PositionComponent, SymbolComponent, PreviousPositionComponent> _dynamicItems;

        private EcsFilter<PositionComponent, SymbolComponent>.Exclude<PreviousPositionComponent, MarkAsRenderedTag> _staticUnrenderedItems;


        public void Init()
        {
            Console.CursorVisible = false;
        }

        public void Run()
        {
            //Console.Clear();
            foreach (var i in _staticUnrenderedItems)
            {
                ref var pos = ref _staticUnrenderedItems.Get1(i);
                ref var ch = ref _staticUnrenderedItems.Get2(i);
                RenderItem(in pos, in ch);

                _staticUnrenderedItems.GetEntity(i).Tag<MarkAsRenderedTag>();
            }

            foreach ( var i in _dynamicItems)
            {
                ref var pos = ref _dynamicItems.Get1(i);
                ref var ch = ref _dynamicItems.Get2(i);
                RenderItem(in pos, in ch);

                ref var prevPos = ref _dynamicItems.Get3(i);
                if (_map.Get(prevPos.Value).IsNull())
                {
                    RenderItem(in prevPos.Value, in SymbolComponent.Empty);
                }

                _dynamicItems.GetEntity(i).Tag<MarkAsRenderedTag>();
            }

            void RenderItem(in PositionComponent pos, in SymbolComponent symbol)
            {

                Console.SetCursorPosition(pos.Value.X, pos.Value.Y);
                Console.Write(symbol.Value);
            }
        }
    }
}
