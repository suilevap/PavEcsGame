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

        private readonly Dictionary<ConsoleKey, PositionComponent>[] _configs;
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<PlayerIndexComponent, IsActiveTag>>
            .Opt<EcsSpec<MoveCommandComponent>> _spec;

        public KeyboardMoveSystem(bool waitKey, TurnManager turnManager, EcsUniverse universe)
        {
            _waitKey = waitKey;
            _turnManager = turnManager;
            universe
                .Register(this)
                .Build(ref _spec);

            _configs = new Dictionary<ConsoleKey, PositionComponent>[]
            {
                new Dictionary<ConsoleKey, PositionComponent>(){
                    { ConsoleKey.UpArrow, new PositionComponent(0, -1) },
                    { ConsoleKey.DownArrow, new PositionComponent(0, 1) },
                    { ConsoleKey.LeftArrow, new PositionComponent(-1, 0) },
                    { ConsoleKey.RightArrow, new PositionComponent(1, 0) }
                }
            };
        }
        public void Init(IEcsSystems systems)
        {
            //var config1 = new Dictionary<ConsoleKey, PositionComponent>() { }
        }

        public void Run(IEcsSystems systems)
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
            var (playerIdPool, _) = _spec.Include;
            var commandPool = _spec.Optional.Pool1;

            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                var playerId = playerIdPool.Get(ent).Index;

                if (playerId >= 0
                    && playerId < _configs.Length
                    && _configs[playerId].TryGetValue(key, out var newSpeed))
                {

                    commandPool.Ensure(ent, out _) =
                            new MoveCommandComponent()
                            {
                                Target = newSpeed,
                                IsRelative = true,
                            };
                }
            }
        }
    }

}
