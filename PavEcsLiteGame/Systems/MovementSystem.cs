using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class MovementSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly Providers _providers;

        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;

        [Entity]
        private readonly partial struct MoveableEnt
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly SpeedComponent Speed();
            public partial ref readonly IsActiveTag ActiveTag();
            public partial OptionalComponent<NewPositionComponent> NewPos();
        }


        public MovementSystem(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }

        public void Init(IEcsSystems systems)
        {
            _reg = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(IEcsSystems systems)
        {
            var hasWorkToDo = false;
            foreach (var entity in _providers.MoveableEntProvider)
                if (entity.Speed().Speed != Int2.Zero)
                {
                    hasWorkToDo = true;
                    entity.NewPos().Ensure().Value = new PositionComponent(entity.Pos().Value + entity.Speed().Speed);
                }

            _reg.UpdateState(hasWorkToDo);
        }
    }
}