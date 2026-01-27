using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class MoveCommandSystem : IEcsInitSystem, IEcsRunSystem, IEcsSystemSpec
    {
        private readonly TurnManager _turnManager;


        private TurnManager.SimSystemRegistration _registration;


        [Entity]
        private partial struct Entity
        {
            public partial RequiredComponent<MoveCommandComponent> Move();
            public partial ref SpeedComponent Speed();

            public partial ref CommandTokenComponent CommandToken();
        }


        public MoveCommandSystem(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }

        public void Init(IEcsSystems systems)
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(IEcsSystems systems)
        {
            _registration.UpdateState(_providers.EntityProvider.Filter);
            foreach (var ent in _providers.EntityProvider)
            {
                ref readonly var command = ref ent.Move().Get();
                if (command.IsRelative) ent.Speed() = new SpeedComponent(command.Target.Value);
                ent.Move().Remove();
                ent.CommandToken().ActionCount--;
            }
        }
    }
}