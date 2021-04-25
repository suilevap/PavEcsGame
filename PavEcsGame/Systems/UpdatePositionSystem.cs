using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.GameLoop;
using PavEcsGame.Components;

namespace PavEcsGame.Systems
{
    class UpdatePositionSystem : IEcsRunSystem
    {
        private IMapData<PositionComponent, EcsEntity> _map;
        private EcsFilter<NewPositionComponent> _filter;

        public void Run()
        {
           foreach (var i in _filter)
            {
                ref var newPos = ref _filter.Get1(i);
                var safeNewPos = _map.GetSafePos(newPos.Value);

                if (_map.Get(safeNewPos).IsNull())
                {
                    var ent = _filter.GetEntity(i);

                    bool hasPos = (ent.Has<PositionComponent>());

                    ref var pos = ref ent.Get<PositionComponent>();
                    ref var prevPos = ref ent.Get<PreviousPositionComponent>();

                    if (hasPos)
                    {
                        _map.Set(pos, EcsEntity.Null);
                        prevPos.Value = pos;
                    }
                    else
                    {
                        prevPos.Value = default;
                    }
                    pos = safeNewPos;
                    _map.Set(pos, ent);
                    ent.Del<NewPositionComponent>();
                }
            }
        }
    }
}
