﻿using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PaveEcsGame;
using System.Diagnostics;

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
        private readonly EcsEntityFactorySpec<EcsSpec<CollisionEvent<EcsPackedEntityWithWorld>>> _collEvenFactorySpec;

        public UpdatePositionSystem(
            TurnManager turnManager, 
            IMapData<PositionComponent, EcsPackedEntityWithWorld> mapData,
            EcsUniverse universe)
        {
            _turnManager = turnManager;
            _map = mapData;

            _collEvenFactorySpec = universe.CreateEntityFactorySpec(
                EcsSpec<CollisionEvent<EcsPackedEntityWithWorld>>.Build()
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
            PlaceToNewPos();
            RemoveFromMap();
            MoveToNewPos();

            void PlaceToNewPos()
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
                                    .Add(_collEvenFactorySpec.Pools, new CollisionEvent<EcsPackedEntityWithWorld>()
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
                var prevPool = _movePosSpec.Optional.Pool1;
                //remove from previous pos anf store previous pos
                foreach (EcsUnsafeEntity ent in _movePosSpec.Filter)
                {

                    ref var pos = ref posPool.Get(ent);
                    ref var mapEnt = ref _map.GetRef(pos);
                    Debug.Assert(mapEnt.IsSame(ent), $"Unexpected ent in previous pos.\n" +
                                                     $" Exp :{_movePosSpec.World.PackEntityWithWorld(ent).ToLogString()}.\n" +
                                                     $" Act : {mapEnt.ToLogString()} ");

                    _map.Set(pos, default);

                    prevPool.Set(ent).Value = pos;
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
