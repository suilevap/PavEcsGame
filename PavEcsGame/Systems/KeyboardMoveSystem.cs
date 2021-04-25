﻿using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Text;
using PavEcsGame.Components;

namespace PavEcsGame.Systems
{
    class KeyboardMoveSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter<KeyboardControlComponent, SpeedComponent> _filter;

        private Dictionary<ConsoleKey, SpeedComponent>[] _configs;
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
            if (_filter.IsEmpty())
                return;

            var key = (Console.KeyAvailable || true) ? Console.ReadKey().Key : default;
            foreach (var i in _filter)
            {
                var id = _filter.Get1(i).Index;
                ref var currentSpeed = ref _filter.Get2(i);

                if (id >= 0
                    && id < _configs.Length
                    && _configs[id].TryGetValue(key, out var newSpeed))
                {
                    currentSpeed = newSpeed;
                }
                else
                {
                    currentSpeed.Speed = new Int2();
                }
            }
        }
    }
}
