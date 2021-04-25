using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PavEcsGame.Systems
{
    class VerifyMapSystem : IEcsRunSystem
    {
        private IMapData<PositionComponent, EcsEntity> _map;
        private EcsFilter<PositionComponent> _filter;

        public void Run()
        {
            foreach(var i in _filter)
            {
                ref var pos = ref _filter.Get1(i);
                Debug.Assert( _map.Get(pos) == _filter.GetEntity(i), $"Not stored entity: {_filter.GetEntity(i)}");
            }

            foreach(var (pos,ent) in _map.GetAll())
            {
                if (!ent.IsNull())
                {
                    Debug.Assert(ent.Has<PositionComponent>() && pos.Value == ent.Get<PositionComponent>().Value,
                        $"Stored in wrong place: {ent}, actual pos:{pos}, exptected: {ent.Get<PositionComponent>()}");
                }
            }
        }
    }
}
