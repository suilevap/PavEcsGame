using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
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

        public bool IsAlive => _world.IsAlive();

        public void Start()
        {
            _world = new EcsWorld();
            //var turnManager = new TurnManager(_world);
            var map = new MapData<EcsPackedEntity>();

            _systems = new EcsSystems(_world, "Root");
            //.InjectByDeclaredType(map)
            //.InjectByDeclaredType(turnManager);

            _systems//var initSystems = new EcsSystems(_world, "Init")
                .Add(new SynchronizationContextSystem())
                .Add(new LoadMapSystem("Data/map1.txt", _world, map))
                ;//.Add(new SpawnSystem());

            //            _systems//var controlSystems = new EcsSystems(_world,"Control")
            //                .Add(new CommandTokenDistributionSystem(TimeSpan.FromSeconds(1f)))
            //                .Add(new KeyboardMoveSystem(waitKey: false))
            //                .Add(new RandomMovementSystem());

            //            _systems//var tickSystems = new EcsSystems(_world,"Tick")   
            //                .Add(new MovementSystem())
            //                .Add(new FrictionSystem());

            //            _systems//var simSystems = new EcsSystems(_world, "Sim")
            //                .Add(new UpdatePositionSystem())
            //#if DEBUG
            //                .Add(new VerifyMapSystem())
            //#endif
            //                .Add(new DamageOnCollisionSystem())
            //                .Add(new DestroyEntitySystem());

            _systems//var renderSystems = new EcsSystems(_world, "Render")
                .Add(new SymbolRenderSystem(map, _world));

            _systems//var cleanupSystems = new EcsSystems(_world, "Cleanup")
                .DelHere<PreviousPositionComponent>()
                .DelHere<TargetCollisionEventComponent<EcsPackedEntity>>()
                .DelHere<SourceCollisionEventComponent<EcsPackedEntity>>();

            _systems
                //.Add(initSystems)
                //.Add(controlSystems)
                //.Add(tickSystems)
                //.Add(simSystems)
                //.Add(renderSystems)
                //.Add(cleanupSystems)
                //.Add(turnManager)
                ////.Add(new SymbolReRenderAllSystem())
                .Init();
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
        }
    }
}
