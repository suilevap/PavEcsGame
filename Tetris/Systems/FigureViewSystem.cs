using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PavEcsGame;
using Tetris.Components;

namespace Tetris.Systems
{
    internal class FigureViewSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<Figure, PositionComponent>>
            .Exc<EcsSpec<ShapeViews>> _figureSpecWithoutViews;

        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<Figure, ShapeViews>> _figureSpec;

        private readonly EcsEntityFactorySpec<EcsSpec<PositionComponent, SymbolComponent>> _viewFactory;

        private readonly struct ShapeViews
        {
            public readonly EcsEntity View1;
            public readonly EcsEntity View2;
            public readonly EcsEntity View3;
            public readonly EcsEntity View4;

            public ShapeViews(EcsEntity view1, EcsEntity view2, EcsEntity view3, EcsEntity view4)
            {
                View1 = view1;
                View2 = view2;
                View3 = view3;
                View4 = view4;
            }
        }

        public FigureViewSystem(EcsUniverse universe)
        {
            universe.Register(this)
                .Build(ref _figureSpecWithoutViews)
                .Build(ref _figureSpec)
                .Build(ref _viewFactory);
        }

        public void Run(IEcsSystems systems)
        {
            var shapePool = _figureSpecWithoutViews.Include.Pool1;
            var viewsPool = _figureSpecWithoutViews.Exclude.Pool1;
            foreach (EcsUnsafeEntity ent in _figureSpecWithoutViews.Filter)
            {
                var shape = shapePool.Get(ent).Shape;
                ref var views = ref viewsPool.Add(ent);
                views = new ShapeViews();
            }
        }
    }
}
