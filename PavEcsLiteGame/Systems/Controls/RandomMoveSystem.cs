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
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class RandomMoveSystem : IEcsRunSystem, IEcsSystemSpec
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
   
        [Entity]
        private partial struct Entity
        {
            public partial ref RandomGeneratorComponent Rnd();
            public partial ref readonly IsActiveTag IsActive();
            public partial OptionalComponent<MoveCommandComponent> Move();

        }

        public RandomMoveSystem(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }

        public void Run(EcsSystems systems)
        {
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;

            foreach (var ent in _providers.EntityProvider)
            {
                var rnd = ent.Rnd().Rnd;
                var newTarget = _moves.GetRandom(rnd);

                ent.Move().Ensure() = new MoveCommandComponent()
                {
                    Target = newTarget,
                    IsRelative = true
                };
            }
        }

    }
}
