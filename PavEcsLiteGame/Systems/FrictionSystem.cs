using System;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    public class FrictionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;
        private readonly EcsFilterSpec<EcsSpec<SpeedComponent, MoveFrictionComponent>, EcsSpec, EcsSpec> _spec;

        public FrictionSystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;
            _spec = universe.CreateFilterSpec(
                EcsSpec<SpeedComponent, MoveFrictionComponent>.Build(),
                EcsSpec.Empty(),
                EcsSpec.Empty()
            );
        }
        public void Init(EcsSystems systems)
        {
            _spec.Init(systems);
            _reg = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(EcsSystems systems)
        {
            bool hasWorkToDo = false;
            var (speedPool, frictionPool) = _spec.Include;
            foreach (var ent in _spec.Filter)
            {
                ref var speed = ref speedPool.Get(ent);
                var friction = frictionPool.Get(ent).FrictionValue;
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