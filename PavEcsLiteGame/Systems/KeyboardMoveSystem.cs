using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Extensions;
using PavEcsGame.Systems.Managers;

namespace PavEcsGame.Systems
{
    class KeyboardMoveSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly bool _waitKey;
        //private EcsFilter<PlayerIndexComponent, SpeedComponent, CommandTokenComponent, IsActiveTag> _filter;

        private readonly TurnManager _turnManager;

        private Dictionary<ConsoleKey, SpeedComponent>[] _configs;
        private readonly EcsFilterSpec<EcsSpec<PlayerIndexComponent, SpeedComponent, CommandTokenComponent, IsActiveTag>, EcsSpec, EcsSpec> _spec;

        public KeyboardMoveSystem(bool waitKey, TurnManager turnManager, EcsUniverse universe)
        {
            _waitKey = waitKey;
            _turnManager = turnManager;
            _spec = universe.CreateFilterSpec(
                EcsSpec<PlayerIndexComponent, SpeedComponent, CommandTokenComponent, IsActiveTag>.Build(),
                EcsSpec.Empty(),
                EcsSpec.Empty()
            );
        }
        public void Init(EcsSystems systems)
        {
            _spec.Init(systems);
            _configs = new Dictionary<ConsoleKey, SpeedComponent>[]
            {
                new Dictionary<ConsoleKey, SpeedComponent>(){
                    { ConsoleKey.UpArrow, new SpeedComponent(0, -1) },
                    { ConsoleKey.DownArrow, new SpeedComponent(0, 1) },
                    { ConsoleKey.LeftArrow, new SpeedComponent(-1, 0) },
                    { ConsoleKey.RightArrow, new SpeedComponent(1, 0) }
                }
            };
            //var config1 = new Dictionary<ConsoleKey, SpeedComponent>() { }
        }

        public void Run(EcsSystems systems)
        {
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;
            if (_spec.Filter.IsEmpty())
                return;
            var key = (_waitKey || Console.KeyAvailable ) 
                ? Console.ReadKey(true).Key 
                : default;

            var (playerIdPool, speedPool, commandTokenPool, _) = _spec.Include;
            foreach (var ent in _spec.Filter)
            {
                var playerId = playerIdPool.Get(ent).Index;
                ref var currentSpeed = ref speedPool.Get(ent);

                if (playerId >= 0
                    && playerId < _configs.Length
                    && _configs[playerId].TryGetValue(key, out var newSpeed))
                {
                    currentSpeed = newSpeed;
                    ref var tokensComponent = ref commandTokenPool.Get(ent);
                    tokensComponent.ActionCount--;
                }
            }
        }
    }
}
