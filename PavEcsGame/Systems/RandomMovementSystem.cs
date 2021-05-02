using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Systems
{
    class RandomMovementSystem : IEcsRunSystem
    {
        private TurnManager _turnManager;

        private EcsFilter<SpeedComponent, RandomGeneratorComponent, IsActiveTag> _filter;
        public void Run()
        {
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;
            foreach (var i in _filter)
            {
                ref var speed = ref _filter.Get1(i);
                var rnd = _filter.Get2(i).Rnd;

                speed.Speed = new Int2(1 - rnd.Next(3), 1 - rnd.Next(3));
            }
        }
    }
}
