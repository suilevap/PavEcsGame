using System;
using System.Diagnostics;
using Leopotam.Ecs;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems.Managers
{
    public class TurnManager : IEcsRunSystem, IEcsSystemSpec
    {

        private long _tick;
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<SystemRefComponent<IEcsSystem>>, EcsSpec<SystemHasMoreWorkTag>>
            /*.Opt<EcsSpec<WaitCommandTokenComponent, CommandTokenComponent>>*/ _tokenSpec;

        private readonly EcsEntityFactorySpec<EcsSpec<SystemRefComponent<IEcsSystem>>> _systemEntityFactorySpec;

        private readonly EcsEntityFactorySpec<EcsSpec<WaitCommandTokenComponent>> _systemEntityFactoryTickSpec;


        public enum Phase
        {
            TickUpdate,
            Simulation
        }

        public TurnManager(EcsUniverse universe)
        {
            universe
                .Register(this)
                .Build(ref _tokenSpec)
                .Build(ref _systemEntityFactorySpec)
                .Build(_systemEntityFactorySpec, ref _systemEntityFactoryTickSpec)
                ;
        }

        public Phase CurrentPhase => _tokenSpec.Filter.IsEmpty() ? Phase.TickUpdate : Phase.Simulation;

        public long Tick => _tick;

        public SimSystemRegistration RegisterSimulationSystem(IEcsSystem system)
        {
            var result = _systemEntityFactorySpec.NewUnsafeEntity()
               .Add(_systemEntityFactorySpec.Pools,
                   new SystemRefComponent<IEcsSystem>() { System = system });
            return new SimSystemRegistration(result, this);
        }

        public TickSystemRegistration RegisterTickSystem(IEcsSystem system)
        {
            var result = _systemEntityFactorySpec.NewUnsafeEntity()
                .Add(_systemEntityFactorySpec.Pools,
                    new SystemRefComponent<IEcsSystem>() { System = system });
            return new TickSystemRegistration(result, this);
        }

        public void Run(IEcsSystems systems)
        {
            if (CurrentPhase == Phase.TickUpdate)
            {
                _tick++;
            }
        }


        private void AssertEntityIsValid(EcsPackedEntityWithWorld entity)
        {
            Debug.Assert(entity.Unpack(out var world, out EcsUnsafeEntity unsafeEnt), $"entity: {entity} should be alive");
            Debug.Assert(_tokenSpec.Include.IsBelongToWorld(world), $"entity doesn't belong to expected world. Actual:{world}, expected:{_tokenSpec.Include}");
            Debug.Assert(_tokenSpec.IncludeReadonly.Pool1.Has(unsafeEnt), $"entity doesn't have expected system component.");
        }

        public readonly struct TickSystemRegistration
        {
            private readonly EcsUnsafeEntity _systemEntity;
            private readonly TurnManager _turnManager;

            public TickSystemRegistration(EcsUnsafeEntity systemEntity, TurnManager manager)
            {
                Debug.Assert(manager._tokenSpec.IncludeReadonly.Pool1.Has(systemEntity), $"entity doesn't have expected system component.");

                _systemEntity = systemEntity;
                _turnManager = manager;

                manager._systemEntityFactoryTickSpec.Pools.Pool1.Add(systemEntity) = new WaitCommandTokenComponent(1);
            }
            //public bool TryGetToken(bool hasWorkToDo)
            //{
            //    if (_turnManager.CurrentPhase == Phase.TickUpdate && hasWorkToDo)
            //    {
            //        var commandTokenPool = _turnManager._tokenSpec.Optional.Pool2;
            //        if (commandTokenPool.Has(_systemEntity))
            //        {

            //            ref var tokens = ref commandTokenPool.Get(_systemEntity);
            //            tokens.ActionCount--;
            //            return true;
            //        }
            //    }
            //    return false;
            //}

            //public bool TryGetToken(EcsFilter mainFilter)
            //{
            //    return TryGetToken(!mainFilter.IsEmpty());
            //}
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
                _systemEntity.TryTag<SystemHasMoreWorkTag>(_manager._tokenSpec.Include.Pool1, hasWorkToDo);
                //_systemEntity.Tag<SystemHasMoreWorkTag>(hasWorkToDo);
            }

            public void UpdateState(EcsFilter mainFilter)
            {
                UpdateState(!mainFilter.IsEmpty());
            }

        }
    }
}