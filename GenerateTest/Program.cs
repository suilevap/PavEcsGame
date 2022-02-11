using System;
using System.Diagnostics;
using Leopotam.EcsLite;
using PavEcsGame;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsGame.Systems;

namespace GenerateTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameMainContainer game = new GameMainContainer();
            game.Start();
            while (game.IsAlive)
            {
                game.Update();
            }
        }
    }

    internal class GameMainContainer
    {
        private readonly EcsWorld _world;
        private readonly EcsSystems _systems;

        public bool IsAlive => _world?.IsAlive() ?? false;

        public GameMainContainer()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, "Root");
        }

        public void Start()
        {
            for (int i = 0; i < 100000; i++)
            {
                var newEnt = _world.NewEntity();
                _world.GetPool<PositionComponent>().Add(newEnt).Value = new (i,1)
                        ;
                if (i % 4 == 0)
                {
                    _world.GetPool<SpeedComponent>().Add(newEnt) = new(i, 1);
                }
            }
            _systems.Add(new EmptySystem(_systems));
            _systems.Add(new TestSystem(_systems));

            _systems
                .Init();
        }


        public void Update()
        {
            _systems.Run();
        }

    }
}
