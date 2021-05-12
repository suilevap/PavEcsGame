using Leopotam.Ecs;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.GameLoop;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    public class DestroyEntitySystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly TurnManager _turnManager;
        //private EcsFilter<DestroyRequestTag>.Exclude<PositionComponent,MarkAsRenderedTag> _destroyFilter;
        //private EcsFilter<PositionComponent, DestroyRequestTag> _removeFromMapFilter;
        private TurnManager.SimSystemRegistration _registration;
        private readonly EcsFilterSpec<EcsSpec<DestroyRequestTag>, EcsSpec, EcsSpec<PositionComponent, MarkAsRenderedTag>> _destroySpec;
        private readonly EcsFilterSpec<EcsSpec<PositionComponent, DestroyRequestTag>, EcsSpec<NewPositionComponent>, EcsSpec> _removeFormMapSpec;

        public DestroyEntitySystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;

            _destroySpec = universe.CreateFilterSpec(
                EcsSpec<DestroyRequestTag>.Build(),
                EcsSpec.Empty(),
                EcsSpec<PositionComponent, MarkAsRenderedTag>.Build()
            );

            _removeFormMapSpec = universe.CreateFilterSpec(
                EcsSpec<PositionComponent, DestroyRequestTag>.Build(),
                EcsSpec<NewPositionComponent>.Build(),
                EcsSpec.Empty()
            );
        }
        public void Init(EcsSystems systems)
        {
            _destroySpec.Init(systems);
            _removeFormMapSpec.Init(systems);
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            _registration.UpdateState(_removeFormMapSpec.Filter);

            var newPosPool = _removeFormMapSpec.Optional.Pool1;

            foreach (EcsUnsafeEntity ent in _removeFormMapSpec.Filter)
            {
                newPosPool.Set(ent).Value = default;
            }

            foreach (var ent in _destroySpec.Filter)
            {
                _destroySpec.World.DelEntity(ent);
            }
        }
    }
}