using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems;
using PavEcsSpec.Generated;

namespace GenerateTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var game = new GameMainContainer();
            game.Start();
            while (game.IsAlive) game.Update();
        }
    }

    internal partial class GameMainContainer
    {
        private readonly Providers _providers;
        private readonly EcsSystems _systems;
        private readonly EcsWorld _world;

        [Entity]
        private partial struct SpawEnt
        {
            public partial ref PositionComponent Pos();
            public partial OptionalComponent<SpeedComponent> Speed();
        }

        public GameMainContainer()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, "Root");
            _providers = new Providers(_systems);
        }

        public bool IsAlive => _world?.IsAlive() ?? false;

        public void Start()
        {
            _systems.Add(new EmptySystem());
            _systems.Add(new TestSystem(_systems));

            _systems
                .Init();

            //var factory = SpawEnt.Create(_systems);

            for (var i = 0; i < 100000; i++)
            {
                //var newEnt = _world.NewEntity();
                //_world.GetPool<PositionComponent>().Add(newEnt).Value = new (i,1)
                var ent = _providers.SpawEntProvider.New();
                ent.Pos().Value = new Int2(i, 1);
                if (i % 4 == 0) ent.Speed().Ensure() = new SpeedComponent(i, 1);
                //_world.GetPool<SpeedComponent>().Add(newEnt) = new(i, 1);
            }
        }


        public void Update()
        {
            _systems.Run();
        }
    }
}