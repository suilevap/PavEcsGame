using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using System.Diagnostics;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    partial class UpdatePositionSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly TurnManager _turnManager;
                 
        private readonly IMapData<PositionComponent, EcsPackedEntityWithWorld> _map;
                 
        private TurnManager.SimSystemRegistration _registration;

        [Entity]
        private readonly partial struct RemoveFromMapEnt
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly NewPositionComponent NewPos();

            public partial ref readonly ColliderComponent Collider();
        }

        [Entity]
        private readonly partial struct NewPosForBodyEnt
        {
            public partial RequiredComponent<NewPositionComponent> NewPos();

            public partial ref readonly ColliderComponent Collider();
        }

        [Entity]
        private readonly partial struct NewPosEnt
        {
            public partial RequiredComponent<NewPositionComponent> NewPos();

            public partial OptionalComponent<PositionComponent> Pos();
        }

        [Entity]
        private readonly partial struct SetPrevPosEnt
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly NewPositionComponent NewPos();

            public partial OptionalComponent<PreviousPositionComponent> PrevPos();
        }

        [Entity(SkipFilter = true)]
        private readonly partial struct CollEventEnt
        {
            public partial ref CollisionEvent<EcsEntity> Col();
        }

        public UpdatePositionSystem(
            TurnManager turnManager, 
            IMapData<PositionComponent, EcsPackedEntityWithWorld> mapData,
            EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
            _map = mapData;
        }

        public void Init(EcsSystems systems)
        {
            _registration = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            _registration.UpdateState(_providers.NewPosEntProvider.Filter);

            //move to new pos in map and solve collide
            PlaceToNewPos();
            RemoveFromMap();
            UpdatePrevPos();
            MoveToNewPos();

            void PlaceToNewPos()
            {
                foreach(var ent in _providers.NewPosForBodyEntProvider)
                {
                    ref var newPosComponent = ref ent.NewPos().Get();
                    if (newPosComponent.Value.TryGet(out var nextPos))
                    {
                        //update new pos to safe one
                        newPosComponent.Value = nextPos = _map.GetSafePos(nextPos);

                        ref var otherEnt = ref _map.GetRef(nextPos);

                        if (!otherEnt.Unpack(out _, out EcsUnsafeEntity otherUnsafeEnt)) //free space
                        {
                            //moved to new pos
                            otherEnt = ent.Id;
                        }
                        else
                        {
                            if (otherUnsafeEnt != ent.GetRawId()) //try to move to same pos
                            {
                                _providers.CollEventEntProvider.New().Col() = new CollisionEvent<EcsEntity>()
                                {
                                    Source = ent.Id,
                                    Target = otherEnt
                                };
                            }

                            ent.NewPos().Remove();
                        }
                    }
                }
            }


            void RemoveFromMap()
            {
                //remove from previous pos anf store previous pos
                foreach (var ent in _providers.RemoveFromMapEntProvider)
                {

                    ref readonly var pos = ref ent.Pos();
                    ref var mapEnt = ref _map.GetRef(pos);
                    Debug.Assert(mapEnt.EqualsTo(ent.Id), $"Unexpected ent in previous pos.\n" +
                                                     $" Exp :{ent.Id.ToLogString()}.\n" +
                                                     $" Act : {mapEnt.ToLogString()} ");

                    _map.Set(pos, default);

                }
            }

            void UpdatePrevPos()
            {
                foreach (var ent in _providers.SetPrevPosEntProvider)
                {
                    ent.PrevPos().Ensure().Value = ent.Pos();
                }

            }
            void MoveToNewPos()
            {
                ////update pos

                foreach (var ent in _providers.NewPosEntProvider)
                {
                    var newPosComponent = ent.NewPos();

                    if (newPosComponent.Get().Value.TryGet(out var nextPos))
                    {
                        ent.Pos().Ensure().Value = nextPos;
                    }
                    else
                    {
                        // move ent to void
                        ent.Pos().Clear();
                    }
                    ent.NewPos().Remove();
                }
            }
        }

    }
}
