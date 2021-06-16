using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.EcsLite;
using PavEcsSpec.EcsLite;
using PaveEcsGame.Utils;

namespace PavEcsGame.Systems
{
    internal class RandomMovementSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly Int2[] _moves = new[] { new Int2(0, 0), new Int2(1, 0), new Int2(-1, 0), new Int2(0, 1), new Int2(0, -1) };

        private readonly TurnManager _turnManager;
        private readonly EcsFilterSpec<
            EcsSpec<SpeedComponent, RandomGeneratorComponent, CommandTokenComponent, IsActiveTag>, 
            EcsSpec, 
            EcsSpec> _spec;


        public RandomMovementSystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;
            universe
                .Register(this)
                .Build(ref _spec);
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

                speed.Speed = _moves.GetRandom(rnd);
                commandTokenPools.Get(ent).ActionCount--;
            }
        }

    }
}
