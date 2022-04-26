using System;
using System.Diagnostics;
using Leopotam.Ecs;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Managers
{
    public partial class TurnManager : IEcsRunSystem, IEcsSystemSpec
    {

        private long _tick;

        [Entity]
        private readonly partial struct TokenEnt
        {
            public partial ref readonly SystemRefComponent<IEcsSystem> System();
            public partial ref SystemHasMoreWorkTag HasMoreWork();

        }

        [Entity(SkipFilter = true)]
        private readonly partial struct AddMoreWorkEnt
        {
            public partial ref readonly SystemRefComponent<IEcsSystem> System();
            public partial OptionalComponent<SystemHasMoreWorkTag> HasMoreWork();

        }

        [Entity(SkipFilter = true)]
        private readonly partial struct SystemEnt
        {
            public partial ref SystemRefComponent<IEcsSystem> System();
        }

        [Entity(SkipFilter = true)]
        private readonly partial struct TickEnt
        {
            public partial ref readonly SystemRefComponent<IEcsSystem> System();

            public partial ref WaitCommandTokenComponent WaitCommand();
        }


        public enum Phase
        {
            TickUpdate,
            Simulation
        }


        public Phase CurrentPhase => _providers.TokenEntProvider.Filter.IsEmpty() ? Phase.TickUpdate : Phase.Simulation;

        public long Tick => _tick;

        public SimSystemRegistration RegisterSimulationSystem(IEcsSystem system)
        {
            var result = _providers.SystemEntProvider.New();
            result.System().System = system;

            return new SimSystemRegistration((EcsUnsafeEntity)result.GetRawId(), this);
        }

        public TickSystemRegistration RegisterTickSystem(IEcsSystem system)
        {
            var result = _providers.SystemEntProvider.New();
            result.System().System = system;

            return new TickSystemRegistration((EcsUnsafeEntity)result.GetRawId(), this);
        }

        public void Run(EcsSystems systems)
        {
            if (CurrentPhase == Phase.TickUpdate)
            {
                _tick++;
            }
        }

        public readonly struct TickSystemRegistration
        {
            private readonly EcsUnsafeEntity _systemEntity;
            private readonly TurnManager _turnManager;

            public TickSystemRegistration(EcsUnsafeEntity systemEntity, TurnManager manager)
            {
                _systemEntity = systemEntity;
                _turnManager = manager;
                var ent = manager._providers.TickEntProvider.TryGetUnsafe(systemEntity);
                Debug.Assert(ent != null, $"entity doesn't have expected system component.");

                ent.Value.WaitCommand() = new WaitCommandTokenComponent(1);
            }
        }

        public readonly struct SimSystemRegistration
        {
            private readonly EcsUnsafeEntity _systemEntity;
            private readonly TurnManager _manager;
            public SimSystemRegistration(EcsUnsafeEntity systemEntity, TurnManager manager)
            {
                _systemEntity = systemEntity;
                _manager = manager;
            }
            public void UpdateState(bool hasWorkToDo)
            {
                var ent = _manager._providers.AddMoreWorkEntProvider.TryGetUnsafe(_systemEntity);
                Debug.Assert(ent != null, $"entity doesn't have expected system component.");
                ent.Value.HasMoreWork().TryTag(hasWorkToDo);
            }

            public void UpdateState(EcsFilter mainFilter)
            {
                UpdateState(!mainFilter.IsEmpty());
            }

        }
    }
}