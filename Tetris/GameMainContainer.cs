using System.Diagnostics;
using PavEcsGame.Components;
using Leopotam.EcsLite;
using PavEcsGame;
using PavEcsGame.Systems.Renders;
using PavEcsSpec.EcsLite;
using Tetris.Components;
using Tetris.Systems;

namespace Tetris
{
    internal class GameMainContainer
    {
        private EcsWorld? _world;
        private EcsUniverse? _universe;
        private EcsSystems? _systems;

        public bool IsAlive => _world?.IsAlive() ?? false;

        public void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, "Root");

            _systems
                .AddUniverse(out var universe);
            _universe = universe;
            var map = new MapData<EcsPackedEntityWithWorld>();
            Int2 screenSize = new Int2(32, 20);
            int mapWidth = 12;
            _systems
                .Add(new GenerateSystem<MapComponent>(universe,
                    ()=> new MapComponent()
                    {
                        Data = GenerateBorder(mapWidth, 20),
                        Width = mapWidth,
                        FullLine = GetFullLine(mapWidth),
                        Border = GetFullLine(mapWidth)
                    }))
                .Add(new GenerateFigureSystem(universe, (int)(DateTime.UtcNow.Ticks & 0xFF)))
                .Add(new KeyboardMoveSystem(universe))
                .Add(new RotateSystem(universe))
                .Add(new MoveSystem(universe))
                //.Add(new MapLineRemoveSystem(universe))
                //.Add(new AttachSystem(universe))
                .Add(new FigureRenderSystem(universe))
                .Add(new MapRenderSystem(universe))
                .Add(new DoubleBufferRenderSystem(universe, screenSize))
                .Add(new ConsoleRenderSystem(universe));
//            var turnManager = new TurnManager(universe);

            //            _systems
            //                .MarkPerf(universe, "start")
            //                .Add(turnManager)
            //                .Add(new LoadMapSystem("Data/map1.txt", universe, map))
            //                .Add(new SpawnEntitySystem(universe))
            //                //.Add(new LoadMapSystem("Data/lightTest.txt", universe, map))
            //                .Add(new TileSystem(universe, map))
            //                ;//.Add(new SpawnSystem());

            //            _systems
            //                .Add(new CommandTokenDistributionSystem(TimeSpan.FromSeconds(1f), universe))
            //                .Add(new KeyboardMoveSystem(waitKey: false, turnManager, universe))
            //                .Add(new RandomMoveSystem(turnManager, universe))
            //                .Add(new MoveCommandSystem(turnManager, universe));

            //            _systems
            //                .Add(new UpdateDirectionBasedOnSpeedSystem(universe))
            //                .Add(new MovementSystem(turnManager, universe))
            //                .Add(new FrictionSystem(turnManager, universe));

            //            _systems
            //                .Add(new RelativePositionSystem(turnManager, universe))
            //                .Add(new UpdatePositionSystem(turnManager, map, universe))

            //#if DEBUG
            //                .Add(new VerifyMapSystem(universe, map))
            //#endif
            //                //.Add(new DamageOnCollisionSystem(universe))
            //                .Add(new DestroyEntitySystem(turnManager, universe))
            //                .Add(new DirectionTileSystem(universe))

            //                .Add(new LightSourceSystems(universe))
            //                .Add(new FieldOfViewSystem(universe, map));
            //            //.Add(new LightSystem(universe, map));

            //            _systems
            //                .Add(new LightRenderSystem(universe))
            //                .Add(new PlayerFieldOfViewSystem(universe))
            //                .Add(new DoubleBufferRenderSystem(universe, map))
            //                .Add(new ConsoleRenderSystem(universe))
            //                //.Add(new SymbolRenderSystem(map, universe))
            //                ;

            //            _systems
            //                .UniDelHere<PreviousPositionComponent>(universe)
            //                //.UniDelHere<NewPositionComponent>(universe)
            //                .UniDelHere<CollisionEvent<EcsEntity>>(universe)
            //                .UniDelHere<MapLoadedEvent>(universe)
            //                ;

            //            _systems
            //                .Add(new ActionSystem(universe, DebugInfo, TimeSpan.FromSeconds(2)));

            _systems
                .Init();

            PrintUniverseInfo(universe);
        }

        private uint GetBorder(int width)
        {
            var offset = 32 - width;
            uint border = ((uint)0x1 << 31) | (uint)(0x1 << offset);
            return border;
        }

        private uint GetFullLine(int width)
        {
            var offset = 32 - width;

            return (0xFFFFFFFF >> offset) << offset;
        }
        private uint[] GenerateBorder(int width, int height)
        {
            uint[] data = new uint[height];
            var fullLine = GetFullLine(width);
            data[0] = fullLine;
            data[^1] = fullLine;

            uint border = GetBorder(width);//((uint)0x1 << 31) | (uint)(0x1 << offset);
            for (int y = 1; y < data.Length - 1; y++)
            {
                data[y] = border;
            }

            return data;
        }

        private void DebugInfo(EcsUniverse universe, IEcsSystems systems)
        {
            var bytes = GC.GetTotalMemory(false);
            Debug.Print("Memory: {0} kb", bytes / 1024);
        }

        private void PrintUniverseInfo(EcsUniverse universe)
        {
            Debug.Print("Universe worlds count: {0}", universe.GetAllKeys().Count());
            foreach (var gr in universe.GetAllWorlds(_systems))
            {
                Debug.Print("world: {0}, components count: {1}", gr.Key, gr.Count());
                Debug.Print("c: {0}", string.Join(Environment.NewLine + "  ", gr.Select(x => x.Name)));
            }
        }

        public void Update()
        {
            _systems?.Run();
        }

        public void Destroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
            }

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }

            if (_universe != null)
            {
                foreach (var gr in _universe.GetAllWorlds(_systems))
                {
                    gr.Key.Destroy();
                }

                _universe = null;
            }

        }

    }
}
