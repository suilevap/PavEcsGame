using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    partial class MovementSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {

        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;

        private Providers _providers;
        [Entity]
        private readonly partial struct MoveableEnt
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly SpeedComponent Speed();
            public partial ref readonly IsActiveTag ActiveTag();
            public partial OptionalComponent<NewPositionComponent> NewPos();
        }


        public MovementSystem(TurnManager turnManager, EcsSystems universe)
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
            bool hasWorkToDo = false;
            foreach (var entity in _providers.MoveableEntProvider)
            {
                if (entity.Speed().Speed != Int2.Zero)
                {
                    hasWorkToDo = true;
                    entity.NewPos().Ensure().Value = new PositionComponent(entity.Pos().Value + entity.Speed().Speed);
                }
            }
            _reg.UpdateState(hasWorkToDo);
        }
    }
}
