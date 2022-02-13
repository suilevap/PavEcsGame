using System;
using System.Diagnostics;
using Leopotam.EcsLite;
using PavEcsGame;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsGame.Systems;
using PavEcsSpec.Generated;

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

    internal partial class GameMainContainer
    {
        private readonly EcsWorld _world;
        private readonly EcsSystems _systems;

        public bool IsAlive => _world?.IsAlive() ?? false;

        public GameMainContainer()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, "Root");
        }

        [Entity]
        private partial struct SpawEnt
        {
            public partial ref PositionComponent Pos();
            public partial OptionalComponent<SpeedComponent> Speed();

            public static partial IEntityFactory<SpawEnt> GetFactory(EcsSystems systems);
        }

        public void Start()
        {

            _systems.Add(new EmptySystem(_systems));
            _systems.Add(new TestSystem(_systems));

            _systems
                .Init();

            var factory = SpawEnt.GetFactory(_systems);

            for (int i = 0; i < 100000; i++)
            {
                //var newEnt = _world.NewEntity();
                //_world.GetPool<PositionComponent>().Add(newEnt).Value = new (i,1)
                var ent = factory.New();
                ent.Pos().Value = new(i, 1);
                if (i % 4 == 0)
                {
                    ent.Speed().Ensure() = new(i, 1);
                    //_world.GetPool<SpeedComponent>().Add(newEnt) = new(i, 1);
                }
            }
        }


        public void Update()
        {
            _systems.Run();
        }

    }
}
