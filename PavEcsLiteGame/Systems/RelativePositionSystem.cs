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
    internal class RelativePositionSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<RelativePositionComponent, LinkToEntityComponent<EcsEntity>>>
            .Opt<EcsSpec<NewPositionComponent, DirectionComponent, PositionComponent>> _spec;

        private readonly EcsEntityFactorySpec<
            EcsReadonlySpec<PositionComponent, DirectionComponent>> _parentSpec;
        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;

        public RelativePositionSystem(TurnManager turnManager, EcsUniverse universe)
        {
            universe
                .Register(this)
                .Build(ref _spec)
                .Build(ref _parentSpec);
            _turnManager = turnManager;
        }

        public void Init(EcsSystems systems)
        {
            _reg = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            var (relPosPool, parentPool) = _spec.Include;
            var (newPosPool, dirPool, posPool) = _spec.Optional;

            var (parentPosPool, parentDirPool) = _parentSpec.Pools;
            bool hasWorkTodo = false;
            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                var parentEnt = parentPool.Get(ent).TargetEntity;

                if (parentEnt.Unpack(out _, out var parentId)
                    && parentPosPool.Has(parentId)
                    && parentDirPool.Has(parentId))
                {
                    ref readonly var relPos = ref relPosPool.Get(ent);
                    var parentPos = parentPosPool.Get(parentId).Value;
                    ref readonly var parentDir = ref parentDirPool.Get(parentId);

                    var newPos = GetPos(relPos, parentPos, parentDir);

                    if (!posPool.Has(ent) || posPool.Get(ent) != newPos)
                    {
                        var isNewPos = newPosPool.Ensure(ent, out _).Value.TrySet(newPos);
                        hasWorkTodo = hasWorkTodo || isNewPos;
                    }
                    var dir = relPos.RelativeDirection.Direction.Rotate(parentDir.Direction);
                    var isNewDir = dirPool.Ensure(ent, out _).TrySet(new DirectionComponent() { Direction = dir });
                    hasWorkTodo = hasWorkTodo || isNewDir;
                }
            }
            _reg.UpdateState(hasWorkTodo);
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
