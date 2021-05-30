using System;
using Leopotam.EcsLite;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    internal class ActionSystem : IEcsRunSystem
    {
        private readonly EcsUniverse _universe;
        private readonly Action<EcsUniverse, EcsSystems> _action;
        private readonly TimeSpan _delayBetweenRuns;
        private DateTime _previousCheck = default;

        public ActionSystem(EcsUniverse universe, Action<EcsUniverse, EcsSystems> action, TimeSpan delayBetweenRuns)
        {
            _universe = universe;
            _action = action;
            _delayBetweenRuns = delayBetweenRuns;
        }

        public void Run(EcsSystems systems)
        {
            var now = DateTime.UtcNow;
            if (now - _previousCheck > _delayBetweenRuns)
            {
                _previousCheck = now;
                _action(_universe, systems);
            }
        }
    }
}
