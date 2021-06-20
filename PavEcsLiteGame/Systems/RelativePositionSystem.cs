using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PaveEcsGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Systems
{
    internal class RelativePositionSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<RelativePositionComponent, LinkToEntityComponent<EcsEntity>>>
            .Opt<EcsSpec<NewPositionComponent, DirectionComponent>> _spec;

        private readonly EcsEntityFactorySpec<
            EcsReadonlySpec<NewPositionComponent, DirectionComponent>> _parentSpec;
        private readonly TurnManager _turnManager;

        public RelativePositionSystem(TurnManager turnManager, EcsUniverse universe)
        {
            universe
                .Register(this)
                .Build(ref _spec)
                .Build(ref _parentSpec);
            _turnManager = turnManager;
        }

        public void Run(EcsSystems systems)
        {
            var (relPosPool, parentPool) = _spec.Include;
            var (newPosPool, dirPool) = _spec.Optional;

            var (parentNewPosPool, parentDirPool) = _parentSpec.Pools;

            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                var parentEnt = parentPool.Get(ent).TargetEntity;

                if (parentEnt.Unpack(out _, out var parentId)
                    && parentNewPosPool.Has(parentId)
                    && parentDirPool.Has(parentId))
                {
                    ref readonly var relPos = ref relPosPool.Get(ent);
                    var parentPos = parentNewPosPool.Get(parentId).Value;
                    ref readonly var parentDir = ref parentDirPool.Get(parentId);

                    newPosPool.Ensure(ent, out _).Value = GetPos(relPos, parentPos, parentDir);

                    dirPool.Ensure(ent, out _).Direction = relPos.RelativeDirection.Direction.Rotate(parentDir.Direction);
                }
            }
        }

        private PositionComponent? GetPos(
            in RelativePositionComponent relPos,
            in PositionComponent? parentPos,
            in DirectionComponent parentDir)
        {
            if (!parentPos.HasValue)
                return default;
            var absPos = relPos.RelativePosition.Value.Rotate(parentDir.Direction);
            return parentPos.Value + new PositionComponent(absPos);
        }
    }
}
