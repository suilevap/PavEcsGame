using System.Diagnostics;
using Leopotam.Ecs;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Extensions;

namespace PavEcsGame.Systems.Managers
{
    public class TurnManager
    {
        private readonly EcsWorld _world;
        private readonly EcsFilter _hasMoreWorkFilter;

        public enum Phase
        {
            TickUpdate,
            Simulation
        }

        public TurnManager(EcsWorld world)
        {
            _world = world;
            _hasMoreWorkFilter = _world.GetFilter(typeof(EcsFilter<SystemHasMoreWorkTag>), true);
        }

        public Phase CurrentPhase => _hasMoreWorkFilter.IsEmpty() ? Phase.TickUpdate : Phase.Simulation;

        public SystemRegistration Register(IEcsSystem system)
        {
             var result = _world.NewEntity()
                .Replace(new SystemRefComponent() { System = system });
            return new SystemRegistration(result);
        }

        public readonly struct SystemRegistration
        {
            private readonly EcsEntity _systemEntity;

            public SystemRegistration(EcsEntity systemEntity)
            {
                Debug.Assert(systemEntity.Has<SystemRefComponent>(), "Invalid update set for non system entity");
                _systemEntity = systemEntity;
            }
            public void UpdateState(bool hasWorkToDo)
            {
                _systemEntity.Tag<SystemHasMoreWorkTag>(hasWorkToDo);
            }

            public void UpdateState(EcsFilter mainFilter)
            {
                UpdateState(!mainFilter.IsEmpty());
            }

        }
    }
}