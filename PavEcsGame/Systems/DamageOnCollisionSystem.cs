using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.Extensions;

namespace PavEcsGame.Systems
{
    class DamageOnCollisionSystem : IEcsRunSystem
    {
        private EcsFilter<TargetCollisionEventComponent<EcsEntity>, IsActiveTag> _filter;

        public void Run()
        {
            foreach (var i in _filter)
            {
                var ent = _filter.GetEntity(i);
                //ent.Get<NewPositionComponent>().Value = default;
                ent.Tag<DestroyRequestTag>();
            }
        }
    }
}
