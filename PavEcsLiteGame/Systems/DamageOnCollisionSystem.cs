using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class DamageOnCollisionSystem : IEcsRunSystem, IEcsSystemSpec
    {
        [Entity]
        private partial struct Entity
        {
            public partial ref readonly CollisionEvent<EcsEntity> Event();
        }

        [Entity(SkipFilter=true)]
        private partial struct DestroyEnt
        {
            public partial ref readonly IsActiveTag IsActive();

            public partial OptionalComponent<DestroyRequestTag> DestroyTag();
        }

        [Entity(SkipFilter = true)]
        private partial struct PlayerEnt
        {
            public partial ref PlayerIndexComponent PlayerId();
        }

        public void Run(EcsSystems systems)
        {
            foreach(var ent in _providers.EntityProvider)
            {
                var otherEnt = ent.Event().Target;
                if (_providers.DestroyEntProvider.TryGet(otherEnt).TryGet(out var destroyEnt))
                {
                    destroyEnt.DestroyTag().TryTag(true);
                }
            }
        }
    }
}
