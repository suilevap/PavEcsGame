using Leopotam.EcsLite;
using PavEcsGame;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using Tetris.Components;

namespace Tetris.Systems;

internal class AttachSystem : IEcsRunSystem, IEcsSystemSpec
{
    private readonly EcsEntityFactorySpec<
        EcsReadonlySpec<PositionComponent>> _parentSpec;

    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<AttachToComponent>>
        .Opt<EcsSpec<PositionComponent>> _childSpec;


    public struct AttachToComponent
    {
        public EcsPackedEntityWithWorld Parent;
        public PositionComponent Offset;
    }


    public AttachSystem(EcsUniverse universe)
    {
        universe.Register(this)
            .Build(ref _parentSpec)
            .Build(ref _childSpec);
    }

    public void Run(IEcsSystems systems)
    {
        var attachToPool = _childSpec.Include.Pool1;
        var posPool = _childSpec.Optional.Pool1;
        var parentPosPool = _parentSpec.Pools.Pool1;

        foreach (EcsUnsafeEntity ent in _childSpec.Filter)
        {
            ref readonly var attachData = ref attachToPool.Get(ent);
            var parentEnt = attachData.Parent;
            if (parentEnt.Unpack(out _, out EcsUnsafeEntity parentId)
                && parentPosPool.Has(parentId))
            {
                ref readonly var parentPos = ref parentPosPool.Get(parentId);
                posPool.Ensure(ent, out _) = parentPos + attachData.Offset;
            }
        }
    }

}


internal class FigureAttachViewSystem : IEcsRunSystem, IEcsSystemSpec
{
    private readonly EcsEntityFactorySpec<
        EcsSpec<AttachSystem.AttachToComponent, SymbolComponent>> _viewFactory;

    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<Figure>>
        .Exc<EcsSpec<ViewAttachedTag>> _spec;


    private struct ViewAttachedTag : ITag { }


    public FigureAttachViewSystem(EcsUniverse universe)
    {
        universe.Register(this)
            .Build(ref _spec)
            .Build(ref _viewFactory);
    }

    public void Run(IEcsSystems systems)
    {
        var figurePool = _spec.Include.Pool1;
        var vieCreatedTagPool = _spec.Exclude.Pool1;

        var (attachPool, symbolPool) = _viewFactory.Pools;

        foreach (EcsUnsafeEntity ent in _spec.Filter)
        {
            ref readonly var figure = ref figurePool.Get(ent);

            //ref readonly var attachData = ref attachToPool.Get(ent);
            //var parentEnt = attachData.Parent;
            //if (parentEnt.Unpack(out _, out EcsUnsafeEntity parentId)
            //    && parentPosPool.Has(parentId))
            //{
            //    ref readonly var parentPos = ref parentPosPool.Get(parentId);
            //    posPool.Ensure(ent, out _) = parentPos + attachData.Offset;
            //}
        }
    }

}