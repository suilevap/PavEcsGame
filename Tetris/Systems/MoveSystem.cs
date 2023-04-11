using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using Tetris.Components;

namespace Tetris.Systems;

internal class MoveSystem : IEcsRunSystem, IEcsSystemSpec
{
    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<DirectionComponent, Figure>, EcsSpec<PositionComponent>>
        .Opt<EcsReadonlySpec<OptionalCommandComponent>> _moveSpec;

    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<MapComponent>> _mapSpec;

    private uint[] _figureData = new uint[4];

    public MoveSystem(EcsUniverse universe)
    {
        universe.Register(this)
            .Build(ref _mapSpec)
            .Build(ref _moveSpec);
    }

    public void Run(IEcsSystems systems)
    {
        var dirPool = _moveSpec.IncludeReadonly.Pool1;
        var figPool = _moveSpec.IncludeReadonly.Pool2;
        var posPool = _moveSpec.Include.Pool1;
        var optCmdPool = _moveSpec.Optional.Pool1;

        var mapPool = _mapSpec.Include.Pool1;

        foreach (EcsUnsafeEntity ent in _moveSpec.Filter)
        {
            ref readonly var dir = ref dirPool.Get(ent);
            ref var pos = ref posPool.Get(ent);
            var newPos = pos.Add(dir.Direction);

            bool isFree = true;
            TetrisMapUtils.FillFigureData(figPool.Get(ent), newPos.Value.X, ref _figureData);
            foreach (EcsUnsafeEntity mapEnt in _mapSpec.Filter)
            {
                isFree &= TetrisMapUtils.IsFree(
                    in mapPool.Get(mapEnt),
                    _figureData,
                    newPos.Value.Y);

            }

            if (isFree)
            {
                pos = newPos;
            }
            else if (!optCmdPool.Has(ent))
            {
                TetrisMapUtils.FillFigureData(figPool.Get(ent), pos.Value.X, ref _figureData);

                foreach (EcsUnsafeEntity mapEnt in _mapSpec.Filter)
                {
                    TetrisMapUtils.Merge(
                        in mapPool.Get(mapEnt),
                        _figureData,
                        pos.Value.Y);
                }
                _moveSpec.World.DelEntity(ent);
            }
        }
    }
}
