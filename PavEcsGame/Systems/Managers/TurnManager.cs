using System;
using System.Diagnostics;
using Leopotam.Ecs;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Extensions;

namespace PavEcsGame.Systems.Managers
{
    public class TurnManager : IEcsRunSystem
    {
        [EcsIgnoreInject]
        private readonly EcsWorld _world;
        [EcsIgnoreInject]
        private readonly EcsFilter _hasMoreWorkFilter;

        private long _tick;

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

        public long Tick => _tick;

        public SimSystemRegistration RegisterSimulationSystem(IEcsSystem system)
        {
            var result = _world.NewEntity()
               .Replace(new SystemRefComponent() { System = system });
            return new SimSystemRegistration(result);
        }

        public TickSystemRegistration RegisterTickSystem(IEcsSystem system)
        {
            var result = _world.NewEntity()
                .Replace(new SystemRefComponent() { System = system });
            return new TickSystemRegistration(result, this);
        }


        public void Run()
        {
            if (CurrentPhase == Phase.TickUpdate)
            {
                _tick++;
            }
        }

        public readonly struct TickSystemRegistration
        {
            private readonly EcsEntity _systemEntity;
            private readonly TurnManager _turnManager;

            public TickSystemRegistration(EcsEntity systemEntity, TurnManager turnManager)
            {
                _turnManager = turnManager;
                Debug.Assert(systemEntity.Has<SystemRefComponent>(), "Invalid update set for non system entity");
                systemEntity.Replace(new WaitCommandTokenComponent(1));
                _systemEntity = systemEntity;
            }
            public bool TryGetToken(bool hasWorkToDo)
            {
                if (_turnManager.CurrentPhase == Phase.TickUpdate && hasWorkToDo)
                {
                    if (_systemEntity.Has<CommandTokenComponent>())
                    {
                        ref var tokens = ref _systemEntity.Get<CommandTokenComponent>();
                        tokens.ActionCount--;
                        return true;
                    }
                }
                return false;
            }

            public bool TryGetToken(EcsFilter mainFilter)
            {
                return TryGetToken(!mainFilter.IsEmpty());
            }
        }

        public readonly struct SimSystemRegistration
        {
            private readonly EcsEntity _systemEntity;

            public SimSystemRegistration(EcsEntity systemEntity)
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