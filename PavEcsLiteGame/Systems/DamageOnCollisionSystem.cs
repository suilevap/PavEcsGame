using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    internal class DamageOnCollisionSystem : IEcsRunSystem
    {
        private EcsFilterSpec<EcsSpec<CollisionEventComponent<EcsPackedEntityWithWorld>>, EcsSpec, EcsSpec> _spec;

        private EcsEntityFactorySpec<EcsSpec<DestroyRequestTag, IsActiveTag>> _destroyFactorySpec;

        public DamageOnCollisionSystem(EcsUniverse universe)
        {
            _spec = universe.CreateFilterSpec(
                EcsSpec<CollisionEventComponent<EcsPackedEntityWithWorld>>.Build(),
                EcsSpec.Empty(),
                EcsSpec.Empty()
            );

            _destroyFactorySpec = universe.CreateEntityFactorySpec(
                EcsSpec<DestroyRequestTag, IsActiveTag>.Build()
            );
        }

        public void Run(EcsSystems systems)
        {
            var (destroyReqPool, isActivePool) = _destroyFactorySpec.Pools;
            var collEventPool = _spec.Include.Pool1;
            foreach (var ent in _spec.Filter)
            {
                var otherEnt = collEventPool.Get(ent).Target;
                if (otherEnt.Unpack(out _, out EcsUnsafeEntity otherId)
                    && isActivePool.Has(otherId))
                {
                    otherId.TryTag<DestroyRequestTag>(destroyReqPool, true);
                }
            }
        }
    }
}
