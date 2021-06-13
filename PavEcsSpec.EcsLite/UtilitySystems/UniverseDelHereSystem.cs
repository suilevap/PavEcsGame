using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    internal sealed class UniverseDelHereSystem<T> : IEcsRunSystem, IEcsSystemSpec
        where T : struct
    {
        private readonly EcsFilterSpec<EcsSpec<T>, EcsSpec, EcsSpec> _spec;

        public UniverseDelHereSystem(EcsUniverse universe)
        {
            universe
                .Register(this)
                .Build(ref _spec);
        }


        public void Run(EcsSystems systems)
        {
            var pool = _spec.Include.Pool1;
            foreach (var entity in _spec.Filter)
            {
                pool.Del(entity);
            }
        }
    }
}