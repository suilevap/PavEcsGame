using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    class UpdateDirectionBasedOnSpeedSystem : IEcsRunSystem, IEcsSystemSpec
    {

        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<SpeedComponent, DirectionBasedOnSpeed, IsActiveTag>, EcsSpec<DirectionComponent>> _spec;

        public UpdateDirectionBasedOnSpeedSystem(EcsUniverse universe)
        {
            universe
                .Register(this)
                .Build(ref _spec);
        }

        public void Run(IEcsSystems systems)
        {
  
            var (speedPool, _, _) = _spec.IncludeReadonly;
            var dirPool = _spec.Include.Pool1;

            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                ref readonly var currentSpeed = ref speedPool.Get(ent);
                if (currentSpeed.Speed != Int2.Zero)
                {
                    ref var dir = ref dirPool.Get(ent);
                    dir.Direction = currentSpeed.Speed;
                }
            }
        }
    }
}
