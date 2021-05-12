using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    internal sealed class UniverseDelHereSystem<T> : IEcsInitSystem, IEcsRunSystem where T : struct
    {
        private readonly EcsFilterSpec<EcsSpec<T>, EcsSpec, EcsSpec> _spec;

        public UniverseDelHereSystem(EcsUniverse universe)
        {
            _spec = universe.CreateFilterSpec(
                EcsSpec<T>.Build(),
                EcsSpec.Empty(),
                EcsSpec.Empty());
        }

        public void Init(EcsSystems systems)
        {
            _spec.Init(systems);
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _spec.Filter) _spec.Include.Pool1.Del(entity);
        }
    }
}