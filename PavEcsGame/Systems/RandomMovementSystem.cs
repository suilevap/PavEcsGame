using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Systems
{
    class RandomMovementSystem : IEcsRunSystem
    {
        private EcsFilter<SpeedComponent, RandomGeneratorComponent> _filter;
        public void Run()
        {
            foreach (var i in _filter)
            {
                ref var speed = ref _filter.Get1(i);
                var rnd = _filter.Get2(i).Rnd;

                speed.Speed = new Int2(1 - rnd.Next(3), 1 - rnd.Next(3));
            }
        }
    }
}
