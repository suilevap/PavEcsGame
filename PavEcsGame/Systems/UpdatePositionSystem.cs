using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.GameLoop;
using PavEcsGame.Components;
using PavEcsGame.Extensions;
using Microsoft.VisualBasic;
using System.Diagnostics;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;
using PaveEcsGame;

namespace PavEcsGame.Systems
{
    class UpdatePositionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private TurnManager _turnManager;

        private IMapData<PositionComponent, EcsEntity> _map;
        private EcsFilter<PositionComponent, NewPositionComponent> _movePosFilter;

        private EcsFilter<NewPositionComponent> _newPosFilter;
        
        [EcsIgnoreInject]
        private EcsEntity _systemEnt;
        private TurnManager.SimSystemRegistration _registration;

        public void Init()
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run()
        {
            _registration.UpdateState(_newPosFilter);

            //move to new pos in map and solve collide
            foreach(var i in _newPosFilter)
            {
                var ent = _newPosFilter.GetEntity(i);
                ref var newPosComponent = ref _newPosFilter.Get1(i);
                if (newPosComponent.Value.TryGet(out var nextPos))
                {
                    //update new pos to safe one
                    newPosComponent.Value = nextPos = _map.GetSafePos(nextPos);
                    
                    ref var otherEnt = ref _map.GetRef(nextPos);
                    if (!otherEnt.IsAlive())//free space
                    {
                        //moved to new pos
                        otherEnt = ent;
                    }
                    else
                    {
                        if (otherEnt != ent) //try to move to same pos
                        {
                            ref var coll = ref ent.Get<SourceCollisionEventComponent<EcsEntity>>();
                            coll.OtherEntity = otherEnt;

                            ref var otherColl = ref otherEnt.Get<TargetCollisionEventComponent<EcsEntity>>();
                            otherColl.OtherEntity = ent;
                        }
                        ent.Del<NewPositionComponent>();
                    }
                }
            }

            //remove from previous pos anf strore previos pos
            foreach(var i in _movePosFilter)
            {
                var ent = _movePosFilter.GetEntity(i);
                ref var pos = ref _movePosFilter.Get1(i);
                Debug.Assert(_map.Get(pos) == ent, "Unexpected ent in previous pos");

                _map.Set(pos, EcsEntity.Null);
                ent.Get<PreviousPositionComponent>().Value = pos;
            }

            //update pos
            foreach(var i in _newPosFilter)
            {
                var ent = _newPosFilter.GetEntity(i);

                ref var newPosComponent = ref _newPosFilter.Get1(i);
                if (newPosComponent.Value.TryGet(out var nextPos))
                {
                    ent.Get<PositionComponent>() = nextPos;
                }
                else
                {
                    // move ent to void
                    ent.Del<PositionComponent>();
                }
                ent.Del<NewPositionComponent>();
            }
        }

    }
}
