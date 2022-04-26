using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using System;
using System.Collections.Generic;
using System.Text;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class RelativePositionSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {

        [Entity]
        private readonly partial struct Ent
        {
            public partial ref readonly RelativePositionComponent RelativePos();
            public partial ref readonly LinkToEntityComponent<EcsEntity> LinkTo();

            public partial OptionalComponent<NewPositionComponent> NewPos();
            public partial OptionalComponent<DirectionComponent> Dir();
            public partial OptionalComponent<PositionComponent> Pos();

        }

        [Entity(SkipFilter = true)]
        private readonly partial struct ParentEntity
        {
            public partial ref readonly PositionComponent Pos();

            public partial ref readonly DirectionComponent Dir();
        }

        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;

        public RelativePositionSystem(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }

        public void Init(EcsSystems systems)
        {
            _reg = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            bool hasWorkTodo = false;
            foreach (var ent in _providers.EntProvider)
            {
                var parentId = ent.LinkTo().TargetEntity;
                if (_providers.ParentEntityProvider.TryGet(parentId).TryGet(out var parentEnt))
                {
                    ref readonly var relPos = ref ent.RelativePos();
                    var newPos = GetPos(relPos, parentEnt.Pos(), parentEnt.Dir());
                    var pos = ent.Pos();
                    if (!pos.Has() || pos.Ensure() != newPos)//todo: add try get for optional?
                    {
                        var isNewPos = ent.NewPos().Ensure().Value.TrySet(newPos);
                        hasWorkTodo = hasWorkTodo || isNewPos;
                    }
                    var dir = relPos.RelativeDirection.Direction.Rotate(parentEnt.Dir().Direction);
                    var isNewDir = ent.Dir().Ensure().TrySet(new DirectionComponent() { Direction = dir });
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
