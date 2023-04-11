using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using Tetris.Components;

namespace Tetris.Systems;

internal class FigureRenderSystem : IEcsRunSystem, IEcsSystemSpec
{
    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<Figure, PositionComponent>> _figureSpec;

    //private readonly EcsEntityFactorySpec<EcsSpec<RenderItemCommand>> _renderCommandFactory;
    private readonly EcsEntityFactorySpec<EcsSpec<PositionComponent, SymbolComponent, OneFrameComponent>> 
        _renderFactory;

    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<OneFrameComponent>> _removeSpec;

    private struct OneFrameComponent : ITag {}

    public FigureRenderSystem(EcsUniverse universe)
    {
        universe.Register(this)
            .Build(ref _figureSpec)
            .Build(ref _renderFactory)
            .Build(ref _removeSpec);
        //.Build(ref _renderCommandFactory);
    }

    public void Run(IEcsSystems systems)
    {
        foreach (EcsUnsafeEntity ent in _removeSpec.Filter)
        {
            _removeSpec.World.DelEntity(ent);
        }
        var shapePool = _figureSpec.Include.Pool1;
        var posPool = _figureSpec.Include.Pool2;
        foreach (EcsUnsafeEntity ent in _figureSpec.Filter)
        {
            var shape = FiguresData.Instance.GetFigureData(shapePool.Get(ent));
            ref readonly var pos = ref posPool.Get(ent);
            
            for (int i = 0; i < 16; i++)
            {
                if ((shape & (1 << i)) != 0)
                {
                    var x = 4 - 1 - (i % 4);
                    var y = 4 - 1 - i / 4;

                    _renderFactory.NewUnsafeEntity()
                        .Add(_renderFactory.Pools,
                            pos.Add(x, y),
                            new SymbolComponent('#', Depth.Foreground),
                            new OneFrameComponent());
                    //_renderCommandFactory.NewUnsafeEntity()
                    //    .Add(_renderCommandFactory.Pools,

                    //        new RenderItemCommand()
                    //        {
                    //            Symbol = new SymbolComponent('#', Depth.Foreground),
                    //            Position = pos.Add(x,y),
                    //        });
                }
            }
        }
    }

}