using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;

namespace PavEcsGame.Systems
{
    class MovementSystem : IEcsRunSystem
    {

        private EcsFilter<PositionComponent, SpeedComponent, IsActiveTag> _filter;

        public void Run()
        {
            foreach (var i in _filter)
            {
                ref var pos = ref _filter.Get1(i);
                ref var speed = ref _filter.Get2(i);
                _filter.GetEntity(i)
                    .Replace(new NewPositionComponent()
                    {
                        Value = new PositionComponent(pos.Value + speed.Speed)
                    });
                //pos.Value += speed.Speed;
            }
        }
    }
}
