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
using PavEcsGame;

namespace PavEcsGame.GameLoop
{
    internal class GameMainContainer
    {
        private EcsWorld _world;
        private EcsUniverse? _universe;
        private EcsSystems _systems;

        public bool IsAlive => _world?.IsAlive() ?? false;

        public GameMainContainer()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, "Root");
        }
        public void Start()
        {

            _systems
                .AddUniverse(out var universe)
                .Add(new SynchronizationContextSystem());
            _universe = universe;
            var map = new MapData<EcsPackedEntityWithWorld>();

            var turnManager = new TurnManager(_systems);

            _systems
                .MarkPerf(universe, "start")
                .Add(turnManager)
                .Add(new LoadMapSystem("Data/map1.txt", _systems, map))
                .Add(new SpawnEntitySystem(_systems))
                //.Add(new LoadMapSystem("Data/lightTest.txt", universe, map))
                .Add(new TileSystem(_systems, map))
                ;//.Add(new SpawnSystem());

            _systems
                .Add(new CommandTokenDistributionSystem(TimeSpan.FromSeconds(1f), _systems))
                .Add(new KeyboardMoveSystem(waitKey: false, turnManager, _systems))
                .Add(new RandomMoveSystem(turnManager, _systems))
                .Add(new MoveCommandSystem(turnManager, _systems));

            _systems
                .Add(new UpdateDirectionBasedOnSpeedSystem(_systems))
                .Add(new MovementSystem(turnManager, _systems))
                .Add(new FrictionSystem(turnManager, _systems));

            _systems
                .Add(new RelativePositionSystem(turnManager, _systems))
                .Add(new UpdatePositionSystem(turnManager, map, _systems))

#if DEBUG
                .Add(new VerifyMapSystem(_systems, map))
#endif
                //.Add(new DamageOnCollisionSystem(universe))
                .Add(new DestroyEntitySystem(turnManager, _systems))
                .Add(new DirectionTileSystem(_systems))

                .Add(new LightSourceSystems(_systems))
                .Add(new FieldOfViewSystem(_systems, map));
            //.Add(new LightSystem(universe, map));

            _systems
                .Add(new LightRenderSystem(_systems))
                .Add(new PlayerFieldOfViewSystem(_systems))
                .Add(new PrepareForRenderSystem(_systems, map))
                .Add(new ConsoleRenderSystem(_systems))
                //.Add(new SymbolRenderSystem(map, universe))
                ;

            _systems
                .MyDelHere<PreviousPositionComponent>()
                //.UniDelHere<NewPositionComponent>(universe)
                .MyDelHere<CollisionEvent<EcsEntity>>()
                .MyDelHere<MapLoadedEvent>()
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
