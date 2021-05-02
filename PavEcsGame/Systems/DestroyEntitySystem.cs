using Leopotam.Ecs;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Extensions;
using PavEcsGame.GameLoop;
using PavEcsGame.Systems.Managers;

namespace PavEcsGame.Systems
{
    public class DestroyEntitySystem : IEcsRunSystem, IEcsInitSystem
    {
        private TurnManager _turnManager;
        private EcsFilter<DestroyRequestTag>.Exclude<PositionComponent>.Exclude<MarkAsRenderedTag> _destroyFilter;
        private EcsFilter<PositionComponent, DestroyRequestTag> _removeFromMapFilter;
        private TurnManager.SimSystemRegistration _registration;

        public void Init()
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run()
        {
            _registration.UpdateState(_removeFromMapFilter);

            foreach (var i in _removeFromMapFilter)
            {
                //_mapData.Set(in _removeFromMapFilter.Get1(i), EcsEntity.Null);
                //move to void
                _removeFromMapFilter.GetEntity(i).Get<NewPositionComponent>().Value = default;
            }

            foreach (var i in _destroyFilter)
            {
                var ent = _destroyFilter.GetEntity(i);
                ent.Destroy();
            }
        }
    }
}