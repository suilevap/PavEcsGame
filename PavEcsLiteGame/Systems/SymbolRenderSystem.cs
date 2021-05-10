using System;
using PavEcsGame.Components;
using PavEcsGame.Extensions;
using Leopotam.EcsLite;
using PavEcsGame.Components.SystemComponents;

namespace PavEcsGame.Systems
{
    public class SymbolRenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntity> _map;

        private readonly EcsFilterSpec<EcsSpec<PreviousPositionComponent>, EcsSpec<MarkAsRenderedTag>, EcsSpec> _clearPrevPosSpec;
        private readonly EcsFilterSpec<EcsSpec<PositionComponent, SymbolComponent>, EcsSpec, EcsSpec<MarkAsRenderedTag>> _updateCurrentPosSpec;


        public SymbolRenderSystem(IReadOnlyMapData<PositionComponent, EcsPackedEntity> map, EcsUniverse universe)
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
            _clearPrevPosSpec.Init(systems);
            _updateCurrentPosSpec.Init(systems);

            Console.CursorVisible = false;
        }

        public void Run(EcsSystems systems)
        {
            var world = _clearPrevPosSpec.World;
            foreach(var ent in _clearPrevPosSpec.Filter)
            {
                ref var prevPos = ref _clearPrevPosSpec.Include.Pool1.Get(ent);
                if (!_map.Get(prevPos.Value).Unpack(world, out var entity))
                {
                    RenderItem(in prevPos.Value, in SymbolComponent.Empty);
                }
                _clearPrevPosSpec.Optional.Pool1.Del(ent);
            }

            foreach(var ent in _updateCurrentPosSpec.Filter)
            {
                ref var pos = ref _updateCurrentPosSpec.Include.Pool1.Get(ent);
                ref var symbol = ref _updateCurrentPosSpec.Include.Pool2.Get(ent);
                RenderItem(in pos, in symbol);
                _updateCurrentPosSpec.Exclude.Pool1.Add(ent);
            }

            static void RenderItem(in PositionComponent pos, in SymbolComponent symbol)
            {
                Console.SetCursorPosition(pos.Value.X, pos.Value.Y);
                Console.Write(symbol.Value);
            }
        }
    }
}
