using System;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;

namespace PavEcsGame.Systems
{
    public class FrictionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private TurnManager _turnManager;
        private EcsFilter<SpeedComponent, MoveFrictionComponent> _filter;
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
                ref var speed = ref _filter.Get1(i);
                var friction = _filter.Get2(i).FrictionValue;
                if (speed.Speed != Int2.Zero && friction != 0)
                {
                    hasWorkToDo = true;
                    if (Math.Abs(speed.Speed.X) > friction)
                    {
                        speed.Speed.X -= Math.Sign(speed.Speed.X) * friction;
                    }
                    else
                    {
                        speed.Speed.X = 0;
                    }
                    if (Math.Abs(speed.Speed.Y) > friction)
                    {
                        speed.Speed.Y -= Math.Sign(speed.Speed.Y) * friction;
                    }
                    else
                    {
                        speed.Speed.Y = 0;
                    }
                }
            }
            _reg.UpdateState(hasWorkToDo);
        }
    }
}