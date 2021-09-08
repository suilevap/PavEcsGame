using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    class KeyboardMoveSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly bool _waitKey;

        private readonly TurnManager _turnManager;

        private Dictionary<ConsoleKey, SpeedComponent>[] _configs;
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<PlayerIndexComponent, IsActiveTag>, EcsSpec<SpeedComponent, CommandTokenComponent>>_spec;

        public KeyboardMoveSystem(bool waitKey, TurnManager turnManager, EcsUniverse universe)
        {
            _waitKey = waitKey;
            _turnManager = turnManager;
            universe
                .Register(this)
                .Build(ref _spec);
            _configs = new Dictionary<ConsoleKey, SpeedComponent>[]
            {
                new Dictionary<ConsoleKey, SpeedComponent>(){
                    { ConsoleKey.UpArrow, new SpeedComponent(0, -1) },
                    { ConsoleKey.DownArrow, new SpeedComponent(0, 1) },
                    { ConsoleKey.LeftArrow, new SpeedComponent(-1, 0) },
                    { ConsoleKey.RightArrow, new SpeedComponent(1, 0) }
                }
            };
        }
        public void Init(EcsSystems systems)
        {
            //var config1 = new Dictionary<ConsoleKey, SpeedComponent>() { }
        }

        public void Run(EcsSystems systems)
        {
            ConsoleKey key = default;
            if (!_waitKey && Console.KeyAvailable)
            {
                key = Console.ReadKey(true).Key;
            }
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;
            if (_spec.Filter.IsEmpty())
                return;
            
            if (_waitKey)
            {
                key = Console.ReadKey(true).Key;
            }

            var (playerIdPool, _) = _spec.IncludeReadonly;
            var (speedPool, commandTokenPool) = _spec.Include;

            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                var playerId = playerIdPool.Get(ent).Index;

                if (playerId >= 0
                    && playerId < _configs.Length
                    && _configs[playerId].TryGetValue(key, out var newSpeed))
                {
                    ref var currentSpeed = ref speedPool.Get(ent);
                    currentSpeed = newSpeed;
                    ref var tokensComponent = ref commandTokenPool.Get(ent);
                    tokensComponent.ActionCount--;
                }
            }
        }
    }

}
