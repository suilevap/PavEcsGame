using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.EcsLite;

namespace PavEcsGame.Extensions
{

    sealed class UniverseDelHereSystem<T> : IEcsInitSystem, IEcsRunSystem where T : struct
    {
        private EcsFilterSpec<EcsSpec<T>, EcsSpec, EcsSpec> _spec;

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
            foreach (var entity in _spec.Filter)
            {
                _spec.Include.Pool1.Del(entity);
            }
        }

    }
}
