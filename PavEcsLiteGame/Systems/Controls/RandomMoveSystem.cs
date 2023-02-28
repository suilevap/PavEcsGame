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
using PavEcsGame.Utils;

namespace PavEcsGame.Systems
{
    internal class RandomMoveSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly PositionComponent[] _moves = new[]
        {
            new PositionComponent(0, 0), 
            new PositionComponent(1, 0), 
            new PositionComponent(-1, 0), 
            new PositionComponent(0, 1), 
            new PositionComponent(0, -1)
        };

        private readonly TurnManager _turnManager;
        private readonly EcsFilterSpec
            .Inc<
                EcsSpec<RandomGeneratorComponent, IsActiveTag>>
            .Opt<EcsSpec<MoveCommandComponent>> _spec;


        public RandomMoveSystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;
            universe
                .Register(this)
                .Build(ref _spec);
        }

        public void Run(IEcsSystems systems)
        {
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;
            var ( rndPool, _) = _spec.Include;
            var commandPool = _spec.Optional.Pool1;
            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                var rnd = rndPool.Get(ent).Rnd;

                var newTarget = _moves.GetRandom(rnd);
                commandPool.Ensure(ent, out _) = new MoveCommandComponent()
                {
                    Target = newTarget,
                    IsRelative = true
                };
            }
        }

    }
}
