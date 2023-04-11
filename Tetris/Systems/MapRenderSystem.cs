using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using Tetris.Components;

namespace Tetris.Systems;

internal class MapRenderSystem : IEcsRunSystem, IEcsSystemSpec
{
    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<MapComponent>> _mapSpec;

    private readonly EcsEntityFactorySpec<EcsSpec<PositionComponent, SymbolComponent, OneFrameComponent>>
        _renderFactory;

    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<OneFrameComponent>> _removeSpec;

    private struct OneFrameComponent : ITag { }

    public MapRenderSystem(EcsUniverse universe)
    {
        universe.Register(this)
            .Build(ref _mapSpec)
            .Build(ref _removeSpec)
            .Build(ref _renderFactory);
    }

    public void Run(IEcsSystems systems)
    {
        foreach (EcsUnsafeEntity ent in _removeSpec.Filter)
        {
            _removeSpec.World.DelEntity(ent);
        }
        var mapPool = _mapSpec.Include.Pool1;
        foreach (EcsUnsafeEntity ent in _mapSpec.Filter)
        {
            ref readonly var map = ref mapPool.Get(ent);
            for (int y = 0; y < map.Data.Length; y++)
            {
                var line = map.Data[y];
                for (int x = 0; x < map.Width; x++)
                {
                    if ((line & ((uint)1<<(31-x))) != 0)
                    {
                        _renderFactory.NewUnsafeEntity()
                            .Add(_renderFactory.Pools,
                                new PositionComponent(x, y),
                                new SymbolComponent('o', Depth.Foreground),
                                new OneFrameComponent());
                    }
                }
            }
        }
    }
}