using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using Tetris.Components;

namespace Tetris.Systems;

internal class RotateSystem : IEcsRunSystem, IEcsSystemSpec
{

    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<RotateComponent, PositionComponent>, EcsSpec<Figure>>
        .Opt<EcsReadonlySpec<OptionalCommandComponent>> _rotateSpec;

    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<MapComponent>> _mapSpec;
    private uint[] _figureData = new uint[4];

    public RotateSystem(EcsUniverse universe)
    {
        universe.Register(this)
            .Build(ref _mapSpec)
            .Build(ref _rotateSpec);
    }
    public void Run(IEcsSystems systems)
    {
        var (rotatePool, posPool) = _rotateSpec.IncludeReadonly;
        var figurePool = _rotateSpec.Include.Pool1;
        var optCmdPool = _rotateSpec.Optional.Pool1;

        var mapPool = _mapSpec.Include.Pool1;

        foreach (EcsUnsafeEntity ent in _rotateSpec.Filter)
        {
            ref var figure = ref figurePool.Get(ent);

            ref readonly var rotation = ref rotatePool.Get(ent);
            ref readonly var pos = ref posPool.Get(ent);

            var newDir = (byte)((figure.Rotation + rotation.RotateDirection + 4) % 4);


            bool isFree = true;
            TetrisMapUtils.FillFigureData(figure.Shape, newDir, pos.Value.X, ref _figureData);
            foreach (EcsUnsafeEntity mapEnt in _mapSpec.Filter)
            {
                isFree &= TetrisMapUtils.IsFree(
                    in mapPool.Get(mapEnt),
                    _figureData,
                    pos.Value.Y);

            }

            if (isFree)
            {
                figure.Rotation = newDir;
            }
            else if (!optCmdPool.Has(ent))
            {
                //todo: nothing?
            }
        }
    }
    //public void Run2(IEcsSystems systems)
    //{
    //    var rotatePool = _spec.IncludeReadonly.Pool1;
    //    var figurePool = _spec.Include.Pool1;
    //    var optCmdPool = _spec.Optional.Pool1;

    //    foreach (EcsUnsafeEntity ent in _spec.Filter)
    //    {
    //        ref readonly var dir = ref rotatePool.Get(ent);
    //        ref var figure = ref figurePool.Get(ent);
    //        figure.Rotation = (byte)((figure.Rotation + dir.RotateDirection + 4) % 4);
    //    }
    //}
}