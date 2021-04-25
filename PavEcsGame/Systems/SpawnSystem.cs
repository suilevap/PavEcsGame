using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;

namespace PavEcsGame.Systems
{
    class SpawnSystem : IEcsRunSystem 
    {
        private EcsWorld _world = null;
        private IMapData<PositionComponent, EcsEntity> _map;

        private EcsFilter<PositionComponent, SpawnRequestComponent> _filter;


        public void Run()
        {
            foreach(var i in _filter)
            {
                ref var pos = ref _filter.Get1(i);
                if (_map.Get(pos).IsNull())
                {
                    //        return false;
                    var ent = _filter.GetEntity(i);
                    _map.Set(pos, ent);
                    ent.Del<SpawnRequestComponent>();
                }

            }
        }


        //public void Init()
        //{
        //    Random rnd = new Random(4);
        //    for (int i=0; i < 40; i++)
        //    {

        //        while (!TrySpawn(GetRandomPos(rnd), '#')) ;
        //    }

        //    while (!TrySpawn(GetRandomPos(rnd), '@'));
        //}

        //private Int2 GetRandomPos(Random rnd)
        //{
        //    return new Int2(rnd.Next(_map.MinPos.X, _map.MaxPos.X), rnd.Next(_map.MinPos.Y, _map.MaxPos.Y));
        //}

        //private bool TrySpawn(Int2 pos, char symbol)
        //{
        //    if (_map.Get(pos) != default)
        //        return false;
        //    var ent = _world.NewEntity();
        //    //ref var posComponent = ref ent.Get<PositionComponent>();
        //    //posComponent.Value = pos;

        //    //ref var viewComponent = ref ent.Get<SymbolComponent>();
        //    //viewComponent.Value = symbol;


        //    ent.Replace(new PositionComponent() { Value = pos })
        //        .Replace(new SymbolComponent() { Value = symbol })
        //        .Replace(new SpeedComponent() { Speed = new Int2(1 - pos.X%3, 1 - pos.Y%3) });
        //    _map.Set(pos, ent);
        //    return true;
        //}
    }
}
