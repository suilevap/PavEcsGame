using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.LeoEcsExtensions;
using PavEcsGame.Systems;


namespace PavEcsGame.GameLoop
{
    class GameMainContainer
    {
        private EcsWorld _world;
        private EcsSystems _systems;
        private IMapData<PositionComponent, EcsEntity> _map;

        public bool IsAlive => _world.IsAlive();

        public void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, "Default World");

            _map = new MapData<EcsEntity>();

            _systems
                .Add(new LoadMapSystem("Data/map1.txt"), "LoadMap")
                .Add(new SpawnSystem())
                .Add(new MovementSystem())
                .Add(new UpdatePositionSystem())
                .Add(new VerifyMapSystem())
                .Add(new SymbolRenderSystem())
                .Add(new KeyboardMoveSystem(waitKey: true))
                .Add(new RandomMovementSystem())
                .OneFrame<PreviousPositionComponent>()
                //.Add(new SymbolReRenderAllSystem())
                .InjectByDeclaredType(_map)
                .Init();
        }

        public void Update()
        {
            _systems.Run();
        }

        public void Destroy()
        {
            _systems.Destroy();
            _world.Destroy();
        }
    }
}
