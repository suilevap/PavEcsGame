using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    partial class MoveCommandSystem : IEcsInitSystem, IEcsRunSystem, IEcsSystemSpec
    {
        private readonly TurnManager _turnManager;


        [Entity]
        private partial struct Entity
        {
            public partial RequiredComponent<MoveCommandComponent> Move();
            public partial ref SpeedComponent Speed();

            public partial ref CommandTokenComponent CommandToken();

        }


        private TurnManager.SimSystemRegistration _registration;


        public MoveCommandSystem(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }

        public void Init(EcsSystems systems)
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            _registration.UpdateState(_providers.EntityProvider.Filter);
            foreach (Entity ent in _providers.EntityProvider)
            {
                ref readonly var command = ref ent.Move().Get();
                if (command.IsRelative)
                {
                    ent.Speed() = new SpeedComponent(command.Target.Value);
                }
                ent.Move().Remove();
                ent.CommandToken().ActionCount--;
            }
        }
    }
}
