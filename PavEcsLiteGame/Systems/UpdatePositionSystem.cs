using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.GameLoop;
using PavEcsGame.Components;
using Microsoft.VisualBasic;
using System.Diagnostics;
using Leopotam.EcsLite;
using PavEcsGame.Components.SystemComponents;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PaveEcsGame;

namespace PavEcsGame.Systems
{
    class UpdatePositionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly TurnManager _turnManager;
                 
        private readonly IMapData<PositionComponent, EcsPackedEntityWithWorld> _map;
                 
        private TurnManager.SimSystemRegistration _registration;
        private readonly EcsFilterSpec<
            EcsSpec<PositionComponent, NewPositionComponent>, 
            EcsSpec<PreviousPositionComponent>,
            EcsSpec> _movePosSpec;
        private readonly EcsFilterSpec<EcsSpec<NewPositionComponent>, EcsSpec<PositionComponent>, EcsSpec> _newPosSpec;
        private readonly EcsEntityFactorySpec<EcsSpec<CollisionEventComponent<EcsPackedEntityWithWorld>>> _collEvenFactorySpec;

        public UpdatePositionSystem(
            TurnManager turnManager, 
            IMapData<PositionComponent, EcsPackedEntityWithWorld> mapData,
            EcsUniverse universe)
        {
            _turnManager = turnManager;
            _map = mapData;

            _collEvenFactorySpec = universe.CreateEntityFactorySpec(
                EcsSpec<CollisionEventComponent<EcsPackedEntityWithWorld>>.Build()
            );

            _movePosSpec = universe
                .StartFilterSpec(
                    EcsSpec<
                        PositionComponent,
                        NewPositionComponent>.Build())
                .Optional(
                    EcsSpec<PreviousPositionComponent>.Build())
                .End();

            _newPosSpec = universe
                .StartFilterSpec(
                    EcsSpec<NewPositionComponent>.Build())
                .Optional(
                    EcsSpec<PositionComponent>.Build())
                .End();
        }

        public void Init(EcsSystems systems)
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            _registration.UpdateState(_newPosSpec.Filter);

            //move to new pos in map and solve collide
            GenerateCollisions();
            RemoveFromMap();
            MoveToNewPos();

            void GenerateCollisions()
            {
                EcsPool<NewPositionComponent> newPosPool = _newPosSpec.Include.Pool1;
                foreach (EcsUnsafeEntity ent in _newPosSpec.Filter)
                {
                    ref var newPosComponent = ref newPosPool.Get(ent);
                    if (newPosComponent.Value.TryGet(out var nextPos))
                    {
                        //update new pos to safe one
                        newPosComponent.Value = nextPos = _map.GetSafePos(nextPos);

                        ref var otherEnt = ref _map.GetRef(nextPos);

                        if (!otherEnt.Unpack(out _, out EcsUnsafeEntity otherUnsafeEnt)) //free space
                        {
                            //moved to new pos
                            otherEnt = _newPosSpec.World.PackEntityWithWorld(ent);
                        }
                        else
                        {
                            if (otherUnsafeEnt != ent) //try to move to same pos
                            {
                                _collEvenFactorySpec.NewUnsafeEntity()
                                    .Add(_collEvenFactorySpec.Pools, new CollisionEventComponent<EcsPackedEntityWithWorld>()
                                    {
                                        Source = _newPosSpec.World.PackEntityWithWorld(ent),
                                        Target = otherEnt
                                    });
                            }

                            newPosPool.Del(ent);
                        }
                    }
                }
            }

            void RemoveFromMap()
            {
                var (posPool, newPosPool) = _movePosSpec.Include;
                //remove from previous pos anf strore previos pos
                foreach (EcsUnsafeEntity ent in _movePosSpec.Filter)
                {
                    //var ent = _movePosFilter.GetEntity(i);
                    //ref var pos = ref _movePosFilter.Get1(i);


                    ref var pos = ref posPool.Get(ent);
                    Debug.Assert(_map.Get(pos).IsSame(ent), "Unexpected ent in previous pos");

                    _map.Set(pos, default);

                    _movePosSpec.Optional.Pool1.Set(ent).Value = pos;
                    //ent.Get<PreviousPositionComponent>().Value = pos;
                }
            }

            void MoveToNewPos()
            {
                //update pos
                var newPosPool = _newPosSpec.Include.Pool1;
                var posPool = _newPosSpec.Optional.Pool1;
                foreach (EcsUnsafeEntity ent in _newPosSpec.Filter)
                {
                    ref var newPosComponent = ref newPosPool.Get(ent);

                    if (newPosComponent.Value.TryGet(out var nextPos))
                    {
                        posPool.Set(ent) = nextPos;
                    }
                    else
                    {
                        // move ent to void
                        posPool.Del(ent);
                    }

                    newPosPool.Del(ent);
                }
            }
        }

    }
}
