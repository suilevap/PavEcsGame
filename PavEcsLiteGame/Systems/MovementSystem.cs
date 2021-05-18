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

namespace PavEcsGame.Systems
{
    class MovementSystem : IEcsRunSystem, IEcsInitSystem
    {

        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;
        private readonly EcsFilterSpec<
            EcsSpec<PositionComponent, SpeedComponent, IsActiveTag>,
            EcsSpec<NewPositionComponent>, 
            EcsSpec> _spec;


        public MovementSystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;
            universe
                .Build(ref _spec);
        }
        public void Init(EcsSystems systems)
        {
            _reg = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            bool hasWorkToDo = false;
            var (posPool, speedPool, _) = _spec.Include;
            var newPosPool = _spec.Optional.Pool1;
            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                ref SpeedComponent speed = ref speedPool.Get(ent);
                if (speed.Speed != Int2.Zero)
                {
                    hasWorkToDo = true;
                    ref PositionComponent pos = ref posPool.Get(ent);

                    newPosPool.Set(
                        ent, 
                        new NewPositionComponent()
                        {
                            Value = new PositionComponent(pos.Value + speed.Speed)
                        });
                }
            }
            _reg.UpdateState(hasWorkToDo);
        }
    }
}
