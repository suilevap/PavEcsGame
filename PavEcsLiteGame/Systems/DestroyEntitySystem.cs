using Leopotam.Ecs;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.GameLoop;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    public partial class DestroyEntitySystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _registration;

        [Entity]
        private partial struct DestroyEnt
        {
            public partial ref readonly DestroyRequestTag DestroyRequest();
            public partial ExcludeComponent<PositionComponent> NoPos();
            public partial ExcludeComponent<MarkAsRenderedTag> NotRendered();

        }

        [Entity]
        private partial struct RemoveFromMapEnt
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly DestroyRequestTag DestroyReq();
            public partial OptionalComponent<NewPositionComponent> NewPos();

        }

        public DestroyEntitySystem(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }
        public void Init(EcsSystems systems)
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            _registration.UpdateState(_providers.RemoveFromMapEntProvider.Filter);

            foreach (var ent in _providers.RemoveFromMapEntProvider)
            {
                ent.NewPos().Ensure().Value = default;
            }

            foreach (var ent in _providers.DestroyEntProvider)
            {
                _providers.DestroyEntProvider._world.DelEntity(ent.GetRawId());
            }
        }
    }
}