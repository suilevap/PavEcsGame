using System;
using PavEcsGame.Components;
using PavEcsGame.Extensions;
using Leopotam.EcsLite;

namespace PavEcsGame.Systems
{
    public class SymbolRenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntity> _map;

        private readonly EcsWorld _world;
        private EcsFilterSpec<EcsSpec<PreviousPositionComponent>, EcsSpec<MarkAsRenderedTag>, EcsSpec> _clearPrevPosSpec;
        private EcsFilterSpec<EcsSpec<PositionComponent, SymbolComponent>, EcsSpec, EcsSpec<MarkAsRenderedTag>> _updateCurrentPosSpec;


        public SymbolRenderSystem(IReadOnlyMapData<PositionComponent, EcsPackedEntity> map, EcsWorld world)
        {
            _map = map;
            _world = world;

            _clearPrevPosSpec = _world.CreateFilterSpec(
                include: EcsSpec<PreviousPositionComponent>.Create,
                optional: EcsSpec<MarkAsRenderedTag>.Create,
                exclude: EcsSpec.CreateEmpty
            );

            _updateCurrentPosSpec = _world.CreateFilterSpec(
                include: EcsSpec<PositionComponent,SymbolComponent>.Create,
                optional: EcsSpec.CreateEmpty,
                exclude: EcsSpec<MarkAsRenderedTag>.Create
            );
        }
        public void Init(EcsSystems systems)
        {
            Console.CursorVisible = false;
        }

        public void Run(EcsSystems systems)
        {
            foreach(var ent in _clearPrevPosSpec.Filter)
            {
                ref var prevPos = ref _clearPrevPosSpec.Include.Pool1.Get(ent);
                if (!_map.Get(prevPos.Value).Unpack(_world, out var entity))
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
