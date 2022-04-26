using System;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    public partial class FrictionSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;

        [Entity]
        private readonly partial struct Ent
        {
            public partial ref SpeedComponent Speed();
            public partial ref readonly MoveFrictionComponent Friction();
        }

        public FrictionSystem(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }
        public void Init(EcsSystems systems)
        {
            _reg = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            bool hasWorkToDo = false;
            foreach (var ent in _providers.EntProvider)
            {
                ref var speed = ref  ent.Speed();
                var friction = ent.Friction().FrictionValue;
                if (speed.Speed != Int2.Zero && friction != 0)
                {
                    hasWorkToDo = true;
                    if (Math.Abs(speed.Speed.X) > friction)
                    {
                        speed.Speed.X -= Math.Sign(speed.Speed.X) * friction;
                    }
                    else
                    {
                        speed.Speed.X = 0;
                    }
                    if (Math.Abs(speed.Speed.Y) > friction)
                    {
                        speed.Speed.Y -= Math.Sign(speed.Speed.Y) * friction;
                    }
                    else
                    {
                        speed.Speed.Y = 0;
                    }
                }
            }
            _reg.UpdateState(hasWorkToDo);
        }
    }
}