using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class UpdateDirectionBasedOnSpeedSystem : IEcsRunSystem, IEcsSystemSpec //, IEcsInitSystem
    {
        [Entity]
        private readonly partial struct Ent
        {
            public partial ref readonly SpeedComponent Speed();

            public partial ref readonly DirectionBasedOnSpeed DirBasedOnSpeed();

            public partial ref readonly IsActiveTag IsActive();

            public partial ref DirectionComponent Dir();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in _providers.EntProvider)
            {
                ref readonly var currentSpeed = ref ent.Speed();
                if (currentSpeed.Speed != Int2.Zero)
                {
                    ref var dir = ref ent.Dir();
                    dir.Direction = currentSpeed.Speed;
                }
            }
        }
    }
}