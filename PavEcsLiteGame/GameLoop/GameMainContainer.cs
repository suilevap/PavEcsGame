using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsGame.Systems;
using PavEcsGame.Systems.Managers;
using PavEcsGame.Systems.Renders;
using PavEcsSpec.EcsLite;
using PaveEcsGame;

namespace PavEcsGame.GameLoop
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
                .AddUniverse(out var universe)
                .Add(new SynchronizationContextSystem());
            _universe = universe;
            var map = new MapData<EcsPackedEntityWithWorld>();

            var turnManager = new TurnManager(universe);

            _systems
                .MarkPerf(universe, "start")
                .Add(turnManager)
                .Add(new LoadMapSystem("Data/map1.txt", universe, map))
                .Add(new SpawnEntitySystem(universe))
                //.Add(new LoadMapSystem("Data/lightTest.txt", universe, map))
                .Add(new TileSystem(universe, map))
                ;//.Add(new SpawnSystem());

            _systems
                .Add(new CommandTokenDistributionSystem(TimeSpan.FromSeconds(1f), universe))
                .Add(new KeyboardMoveSystem(waitKey: false, turnManager, universe))
                .Add(new RandomMovementSystem(turnManager, universe));

            _systems
                .Add(new UpdateDirectionBasedOnSpeedSystem(universe))
                .Add(new MovementSystem(turnManager, universe))
                .Add(new FrictionSystem(turnManager, universe));

            _systems
                .Add(new RelativePositionSystem(turnManager, universe))
                .Add(new UpdatePositionSystem(turnManager, map, universe))

#if DEBUG
                .Add(new VerifyMapSystem(universe, map))
#endif
                //.Add(new DamageOnCollisionSystem(universe))
                .Add(new DestroyEntitySystem(turnManager, universe))
                .Add(new DirectionTileSystem(universe))

                .Add(new LightSourceSystems(universe))
                .Add(new FieldOfViewSystem(universe, map));
            //.Add(new LightSystem(universe, map));

            _systems
                .Add(new LightRenderSystem(universe))
                .Add(new PlayerFieldOfViewSystem(universe))
                .Add(new PrepareForRenderSystem(universe, map))
                .Add(new ConsoleRenderSystem(universe))
                //.Add(new SymbolRenderSystem(map, universe))
                ;

            _systems
                .UniDelHere<PreviousPositionComponent>(universe)
                //.UniDelHere<NewPositionComponent>(universe)
                .UniDelHere<CollisionEvent<EcsEntity>>(universe)
                .UniDelHere<MapLoadedEvent>(universe)
                ;

            _systems
                .Add(new ActionSystem(universe, DebugInfo, TimeSpan.FromSeconds(2)));

            _systems
                .Init();

            PrintUniverseInfo(universe);
        }

        private void DebugInfo(EcsUniverse universe, EcsSystems systems)
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
