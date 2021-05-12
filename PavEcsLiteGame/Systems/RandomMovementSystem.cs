﻿using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.EcsLite;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    class RandomMovementSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly TurnManager _turnManager;
        private readonly EcsFilterSpec<EcsSpec<SpeedComponent, RandomGeneratorComponent, CommandTokenComponent, IsActiveTag>, EcsSpec, EcsSpec> _spec;


        public RandomMovementSystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;
            _spec = universe.CreateFilterSpec(
                EcsSpec<SpeedComponent, RandomGeneratorComponent, CommandTokenComponent, IsActiveTag>.Build(),
                EcsSpec.Empty(),
                EcsSpec.Empty()
                );
        }
        public void Init(EcsSystems systems)
        {
            _spec.Init(systems);
        }

        public void Run(EcsSystems systems)
        {
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;
            var (speedPool, rndPool, commandTokenPools, _) = _spec.Include;
            foreach (var ent in _spec.Filter)
            {
                ref var speed = ref speedPool.Get(ent);
                var rnd = rndPool.Get(ent).Rnd;

                speed.Speed = new Int2(1 - rnd.Next(3), 1 - rnd.Next(3));
                commandTokenPools.Get(ent).ActionCount--;
            }
        }

    }
}
