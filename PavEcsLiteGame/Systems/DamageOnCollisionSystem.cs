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
        private EcsFilterSpec<
            EcsSpec<CollisionEvent<EcsPackedEntityWithWorld>>,
            EcsSpec, 
            EcsSpec> _spec;

        private EcsEntityFactorySpec<EcsSpec<DestroyRequestTag, IsActiveTag>> _destroyFactorySpec;
        private EcsEntityFactorySpec<EcsSpec<PlayerIndexComponent>> _playerFactorySpec;

        public DamageOnCollisionSystem(EcsUniverse universe)
        {
            universe
                .Build(ref _spec)
                .Build(ref _playerFactorySpec)
                .Build(ref _destroyFactorySpec);
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
