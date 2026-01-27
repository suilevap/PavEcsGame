using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsGame.Utils;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class RandomMoveSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly PositionComponent[] _moves =
        {
            new(0, 0),
            new(1, 0),
            new(-1, 0),
            new(0, 1),
            new(0, -1)
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

        public void Run(IEcsSystems systems)
        {
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;

            foreach (var ent in _providers.EntityProvider)
            {
                var rnd = ent.Rnd().Rnd;
                var newTarget = _moves.GetRandom(rnd);

                ent.Move().Ensure() = new MoveCommandComponent
                {
                    Target = newTarget,
                    IsRelative = true
                };
            }
        }
    }
}