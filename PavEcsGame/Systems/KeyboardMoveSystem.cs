﻿using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Text;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;

namespace PavEcsGame.Systems
{
    class KeyboardMoveSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly bool _waitKey;
        private EcsFilter<PlayerIndexComponent, SpeedComponent, CommandTokenComponent, IsActiveTag> _filter;

        private TurnManager _turnManager;

        private Dictionary<ConsoleKey, SpeedComponent>[] _configs;

        public KeyboardMoveSystem(bool waitKey)
        {
            _waitKey = waitKey;
        }
        public void Init()
        {
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

        public void Run()
        {
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;
            if (_filter.IsEmpty())
                return;
            var key = (_waitKey || Console.KeyAvailable ) ? Console.ReadKey(true).Key : default;
            //var key =  (ConsoleKey)(!_waitKey ? Console.In.Peek(): Console.In.Read());
            foreach (var i in _filter)
            {
                var id = _filter.Get1(i).Index;
                ref var currentSpeed = ref _filter.Get2(i);

                if (id >= 0
                    && id < _configs.Length
                    && _configs[id].TryGetValue(key, out var newSpeed))
                {
                    currentSpeed = newSpeed;
                    ref var tokensComponent = ref _filter.Get3(i);
                    tokensComponent.ActionCount--;
                }
                // else
                // {
                //     currentSpeed.Speed = new Int2();
                // }
            }
        }
    }
}
