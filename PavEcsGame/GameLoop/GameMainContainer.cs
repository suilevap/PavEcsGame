using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.Extensions;
using PavEcsGame.Systems;
using PavEcsGame.Utils;

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
                .Add(new SynchronizationContextSystem())
                .Add(new LoadMapSystem("Data/map1.txt"))
                .Add(new SpawnSystem())
                .Add(new KeyboardMoveSystem(waitKey: true))
                .Add(new RandomMovementSystem())
                .Add(new MovementSystem())
                .Add(new UpdatePositionSystem())
                .Add(new DamageOnCollisionSystem())
#if DEBUG
                .Add(new VerifyMapSystem())
#endif
                .Add(new SymbolRenderSystem())

                .Add(new DestroyEntitySystem())

                .OneFrame<PreviousPositionComponent>()
                .OneFrame<TargetCollisionEventComponent>()
                .OneFrame<SourceCollisionEventComponent>()
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
