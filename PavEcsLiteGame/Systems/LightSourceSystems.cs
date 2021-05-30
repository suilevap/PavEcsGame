using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    internal class LightSourceSystems : IEcsRunSystem
    {
        private readonly EcsFilterSpec<
            EcsSpec<LightSourceComponent>,
            EcsSpec<FieldOfViewRequestEvent>,
            EcsSpec> _lightsSpec;

        private readonly EcsFilterSpec<
             EcsSpec<PlayerIndexComponent, VisualSensorComponent>,
             EcsSpec<FieldOfViewRequestEvent>,
             EcsSpec> _playerSpec;

        public LightSourceSystems(EcsUniverse universe)
        {
            universe
                .Build(ref _playerSpec)
                .Build(ref _lightsSpec);

        }

        public void Run(EcsSystems systems)
        {
            var lightDataPool = _lightsSpec.Include.Pool1;
            var eventPool = _lightsSpec.Optional.Pool1;

            foreach (EcsUnsafeEntity ent in _lightsSpec.Filter)
            {
                var radius = lightDataPool.Get(ent).Radius;
                ref var ev = ref eventPool.Ensure(ent, out var isNew);
                if (isNew || ev.Radius < radius)
                {
                    ev.Radius = radius;
                }
            }

            var (_, sensorPool) = _playerSpec.Include;
            foreach (EcsUnsafeEntity ent in _playerSpec.Filter)
            {
                var radius = sensorPool.Get(ent).Radius;
                ref var ev = ref eventPool.Ensure(ent, out var isNew);
                ev.Radius = radius;
            }
        }
    }
}
