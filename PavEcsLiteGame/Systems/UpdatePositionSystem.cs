using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PaveEcsGame;
using System.Diagnostics;

namespace PavEcsGame.Systems
{
    class UpdatePositionSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly TurnManager _turnManager;
                 
        private readonly IMapData<PositionComponent, EcsPackedEntityWithWorld> _map;
                 
        private TurnManager.SimSystemRegistration _registration;
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<PositionComponent, NewPositionComponent, ColliderComponent>> _removeFromMapSpec;

        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<ColliderComponent>,EcsSpec<NewPositionComponent>> _newPosForBodySpec;

        private readonly EcsFilterSpec
            .Inc<EcsSpec<NewPositionComponent>>
            .Opt<EcsSpec<PositionComponent>> _newPosSpec;

        private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<PositionComponent, NewPositionComponent>>
            .Opt<EcsSpec<PreviousPositionComponent>> _setPrevPosSpec;

        private readonly EcsEntityFactorySpec<
            EcsSpec<CollisionEvent<EcsEntity>>> _collEvenFactorySpec;

        public UpdatePositionSystem(
            TurnManager turnManager, 
            IMapData<PositionComponent, EcsPackedEntityWithWorld> mapData,
            EcsUniverse universe)
        {
            _turnManager = turnManager;
            _map = mapData;

            universe
                .Register(this)
                .Build(ref _newPosForBodySpec)
                .Build(ref _collEvenFactorySpec)
                .Build(ref _removeFromMapSpec)
                .Build(ref _setPrevPosSpec)
                .Build(ref _newPosSpec);
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
            UpdatePrevPos();
            MoveToNewPos();

            void PlaceToNewPos()
            {
                EcsPool<NewPositionComponent> newPosPool = _newPosForBodySpec.Include.Pool1;
                foreach (EcsUnsafeEntity ent in _newPosForBodySpec.Filter)
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
                                    .Add(_collEvenFactorySpec.Pools, new CollisionEvent<EcsEntity>()
                                    {
                                        Source = ent.Pack(_newPosSpec.World),
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
                var (posPool, newPosPool, _) = _removeFromMapSpec.Include;
                //remove from previous pos anf store previous pos
                foreach (EcsUnsafeEntity ent in _removeFromMapSpec.Filter)
                {

                    ref readonly var pos = ref posPool.Get(ent);
                    ref var mapEnt = ref _map.GetRef(pos);
                    Debug.Assert(mapEnt.IsSame(ent), $"Unexpected ent in previous pos.\n" +
                                                     $" Exp :{_removeFromMapSpec.World.PackEntityWithWorld(ent).ToLogString()}.\n" +
                                                     $" Act : {mapEnt.ToLogString()} ");

                    _map.Set(pos, default);

                }
            }

            void UpdatePrevPos()
            {
                var (posPool, _) = _setPrevPosSpec.Include;
                var prevPool = _setPrevPosSpec.Optional.Pool1;

                foreach (EcsUnsafeEntity ent in _setPrevPosSpec.Filter)
                {
                    prevPool.Ensure(ent, out _).Value = posPool.Get(ent);
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
                        posPool.Set(ent, nextPos);
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
