using Leopotam.Ecs;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;

namespace PavEcsGame.Systems
{
    public class DestroyEntitySystem : IEcsRunSystem
    {
        private EcsFilter<DestroyRequestTag> _destroyFilter;
        private IMapData<PositionComponent, EcsEntity> _mapData;
        private EcsFilter<PositionComponent, DestroyRequestTag> _removeFromMapFilter;
        
        public void Run()
        {
            foreach (var i in _removeFromMapFilter)
            {
                _mapData.Set(in _removeFromMapFilter.Get1(i), EcsEntity.Null);
            }

            foreach (var i in _destroyFilter)
            {
                var ent = _destroyFilter.GetEntity(i);
                ent.Destroy();
            }
        }
    }
}