using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Extensions;

namespace PavEcsGame.Systems
{
    class DamageOnCollisionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilterSpec<EcsSpec<CollisionEventComponent<EcsPackedEntityWithWorld>>, EcsSpec, EcsSpec> _spec;

        private EcsEntityFactorySpec<EcsSpec<DestroyRequestTag, IsActiveTag>> _destroyFactorySpec;
        //private EcsFilter<TargetCollisionEventComponent<EcsEntity>, IsActiveTag> _filter;


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
        public void Init(EcsSystems systems)
        {
            _spec.Init(systems);
            _destroyFactorySpec.Init(systems);
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
