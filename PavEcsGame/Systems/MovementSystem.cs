using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;

namespace PavEcsGame.Systems
{
    class MovementSystem : IEcsRunSystem, IEcsInitSystem
    {

        private EcsFilter<PositionComponent, SpeedComponent, IsActiveTag> _filter;
        private TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;

        public void Init()
        {
            _reg = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run()
        {
            bool hasWorkToDo = false;
            foreach (var i in _filter)
            {
                ref var speed = ref _filter.Get2(i);
                if (speed.Speed != Int2.Zero)
                {
                    hasWorkToDo = true;
                    ref var pos = ref _filter.Get1(i);
                    _filter.GetEntity(i)
                        .Replace(new NewPositionComponent()
                        {
                            Value = new PositionComponent(pos.Value + speed.Speed)
                        });
                }
                //pos.Value += speed.Speed;
            }
            _reg.UpdateState(hasWorkToDo);
        }
    }
}
