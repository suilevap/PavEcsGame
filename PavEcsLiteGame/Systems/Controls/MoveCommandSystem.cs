using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    class MoveCommandSystem : IEcsInitSystem, IEcsRunSystem, IEcsSystemSpec
    {
        private readonly TurnManager _turnManager;

        private readonly EcsFilterSpec
            .Inc<EcsSpec<MoveCommandComponent, SpeedComponent, CommandTokenComponent>> _spec;

        private TurnManager.SimSystemRegistration _registration;


        public MoveCommandSystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;
            universe
                .Register(this)
                .Build(ref _spec);
        }

        public void Init(IEcsSystems systems)
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(IEcsSystems systems)
        {
            _registration.UpdateState(_spec.Filter);
            var (commandPool,speedPool, commandTokenPool) = _spec.Include;
            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                ref readonly var command = ref commandPool.Get(ent);

                if (command.IsRelative)
                {
                    speedPool.Ensure(ent, out _) = new SpeedComponent(command.Target.Value);
                }

                commandPool.Del(ent);

                commandTokenPool.Get(ent).ActionCount--;
            }
        }
    }
}
