using System;
using PavEcsGame.Components;
using Leopotam.EcsLite;
using PavEcsGame.Components.SystemComponents;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    public class SymbolRenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        private readonly EcsFilterSpec<EcsSpec<PreviousPositionComponent>, EcsSpec<MarkAsRenderedTag>, EcsSpec> _clearPrevPosSpec;
        private readonly EcsFilterSpec<EcsSpec<PositionComponent, SymbolComponent>, EcsSpec, EcsSpec<MarkAsRenderedTag>> _updateCurrentPosSpec;

        public SymbolRenderSystem(IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> map, EcsUniverse universe)
        {
            _map = map;

            _clearPrevPosSpec = universe.CreateFilterSpec(
                include: EcsSpec<PreviousPositionComponent>.Build(),
                optional: EcsSpec<MarkAsRenderedTag>.Build(),
                exclude: EcsSpec.Empty()
            );

            _updateCurrentPosSpec = universe.CreateFilterSpec(
                include: EcsSpec<PositionComponent, SymbolComponent>.Build(),
                optional: EcsSpec.Empty(),
                exclude: EcsSpec<MarkAsRenderedTag>.Build()
            );
        }
        public void Init(EcsSystems systems)
        {

            Console.CursorVisible = false;
        }

        public void Run(EcsSystems systems)
        {
            ClearPreviousPos();

            RenderInNewPos();

            static void RenderItem(in PositionComponent pos, in SymbolComponent symbol)
            {
                Console.SetCursorPosition(pos.Value.X, pos.Value.Y);
                Console.Write(symbol.Value);
            }

            void ClearPreviousPos()
            {
                var prevPosPool = _clearPrevPosSpec.Include.Pool1;
                var markAsRenderedTagPool = _clearPrevPosSpec.Optional.Pool1;
                foreach (var ent in _clearPrevPosSpec.Filter)
                {
                    ref var prevPos = ref prevPosPool.Get(ent);
                    if (!_map.Get(prevPos.Value).IsAlive())
                    {
                        RenderItem(in prevPos.Value, in SymbolComponent.Empty);
                    }

                    markAsRenderedTagPool.Del(ent);
                }
            }

            void RenderInNewPos()
            {
                var (posPool, symbolPool) = _updateCurrentPosSpec.Include;
                var markAsRenderedTagPool = _updateCurrentPosSpec.Exclude.Pool1;
                foreach (var ent in _updateCurrentPosSpec.Filter)
                {
                    ref var pos = ref posPool.Get(ent);
                    ref var symbol = ref symbolPool.Get(ent);
                    RenderItem(in pos, in symbol);
                    markAsRenderedTagPool.Add(ent);
                }
            }
        }
    }
}
