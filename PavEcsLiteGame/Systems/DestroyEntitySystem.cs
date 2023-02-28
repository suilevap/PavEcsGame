using Leopotam.Ecs;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.GameLoop;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    public class DestroyEntitySystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _registration;
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<DestroyRequestTag>>
            .Exc<EcsReadonlySpec<PositionComponent, MarkAsRenderedTag>> _destroySpec;
        
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<PositionComponent, DestroyRequestTag>>
            .Opt<EcsSpec<NewPositionComponent>> _removeFormMapSpec;

        public DestroyEntitySystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;
            universe
                .Register(this)
                .Build(ref _destroySpec)
                .Build(ref _removeFormMapSpec);
        }
        public void Init(IEcsSystems systems)
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(IEcsSystems systems)
        {
            _registration.UpdateState(_removeFormMapSpec.Filter);

            var newPosPool = _removeFormMapSpec.Optional.Pool1;

            foreach (EcsUnsafeEntity ent in _removeFormMapSpec.Filter)
            {
                newPosPool.Ensure(ent, out _).Value = default;
            }

            foreach (var ent in _destroySpec.Filter)
            {
                _destroySpec.World.DelEntity(ent);
            }
        }
    }
}