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
using PavEcsSpec.Generated;


namespace PavEcsGame.Systems
{
    partial class KeyboardMoveSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly bool _waitKey;

        private readonly TurnManager _turnManager;

        private readonly Dictionary<ConsoleKey, PositionComponent>[] _configs;

        [Entity]
        private readonly partial struct PlayerEnt
        {
            public partial ref readonly PlayerIndexComponent Player();
            public partial ref readonly IsActiveTag ActiveTag();
            public partial OptionalComponent<MoveCommandComponent> MoveCommand();
        }

        public KeyboardMoveSystem(bool waitKey, TurnManager turnManager, EcsSystems universe)
            :this(universe)
        {
            _waitKey = waitKey;
            _turnManager = turnManager;

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

        public void Run(EcsSystems systems)
        {
            ConsoleKey key = default;
            if (!_waitKey && Console.KeyAvailable)
            {
                key = Console.ReadKey(true).Key;
            }
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;
            if (_providers.PlayerEntProvider.Filter.IsEmpty())
                return;
            
            if (_waitKey)
            {
                key = Console.ReadKey(true).Key;
            }

            foreach (PlayerEnt ent in _providers.PlayerEntProvider)
            {
                var playerId = ent.Player().Index;
                if (playerId >= 0
                  && playerId < _configs.Length
                  && _configs[playerId].TryGetValue(key, out var newSpeed))
                {

                    ent.MoveCommand().Ensure() =
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
