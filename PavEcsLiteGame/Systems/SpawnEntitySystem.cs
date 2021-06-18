using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    internal class SpawnEntitySystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly EcsEntityFactorySpec<
                EcsSpec<IsActiveTag>> _commonFactory;

        private readonly EcsEntityFactorySpec<
            EcsSpec<
                RandomGeneratorComponent,
                SpeedComponent,
                SymbolComponent,
                MoveFrictionComponent,
                WaitCommandTokenComponent>> _enemyFactory;

        private readonly EcsEntityFactorySpec<
            EcsSpec<PlayerIndexComponent,
                SpeedComponent,
                SymbolComponent,
                MoveFrictionComponent,
                WaitCommandTokenComponent>> _playerFactory;

        private readonly EcsEntityFactorySpec<EcsSpec<SymbolComponent, TileComponent>> _wallFactory;
        private readonly EcsEntityFactorySpec<EcsSpec<LightSourceComponent, PositionComponent>> _lightSourceFactory;
        private readonly EcsEntityFactorySpec<EcsSpec<VisualSensorComponent>> _actorFactory;

        private readonly EcsEntityFactorySpec<EcsSpec<DirectionComponent, DirectionTileComponent, DirectionBasedOnSpeed>> _dirTileFactory;

        private readonly EcsFilterSpec
            .Inc<EcsSpec<SpawnRequestComponent>> _spawnResuestSpec;
        private readonly Random _rnd;

        public SpawnEntitySystem(EcsUniverse universe)
        {
            _rnd = new Random(42);
            universe
                .Register(this)
                .Build(ref _spawnResuestSpec)
                .Build(ref _commonFactory)
                .Build(_commonFactory, ref _actorFactory)
                .Build(_actorFactory, ref _playerFactory)
                .Build(_commonFactory, ref _lightSourceFactory)
                .Build(_actorFactory, ref _enemyFactory)
                .Build(_commonFactory, ref _wallFactory)
                .Build(_commonFactory, ref _dirTileFactory);
        }

        public void Run(EcsSystems systems)
        {
            var spawnReqPool = _spawnResuestSpec.Include.Pool1;

            foreach (EcsUnsafeEntity ent in _spawnResuestSpec.Filter)
            {
                ref var spawnReq = ref spawnReqPool.Get(ent);
                TrySpawnEntity(ent, spawnReq, _rnd);
                ent.Add(
                    _commonFactory.Pools,
                    new IsActiveTag());

                spawnReqPool.Del(ent);
            }
        }

        private void TrySpawnEntity(EcsUnsafeEntity ent, in SpawnRequestComponent request, Random rnd)
        {
            //EcsUnsafeEntity ent;
            //if (!entity.Unpack(out var world, out ent))
            //    return;

            switch (request.Type)
            {
                case EntityType.Wall:
                    //Debug.Assert(_wallFactory.IsBelongToWorld(world) );
                    ent.Add(_wallFactory.Pools,
                            new SymbolComponent
                            {
                                Value = '#',
                                Depth = Depth.Foreground,
                                MainColor = ConsoleColor.Gray
                            },
                            new TileComponent() { RuleName = "wall_rule" });
                    break;
                case EntityType.Player:
                    //Debug.Assert(_playerFactory.IsBelongToWorld(world));

                    ent.Add(_playerFactory.Pools,
                            new PlayerIndexComponent { Index = 0 },
                            new SpeedComponent(),
                            new SymbolComponent
                            {
                                Value = '@',
                                Depth = Depth.Foreground,
                                MainColor = ConsoleColor.White
                            },
                            new MoveFrictionComponent { FrictionValue = 1 },
                            new WaitCommandTokenComponent(1))
                        .Add(_actorFactory.Pools,
                            new VisualSensorComponent() { Radius = 16 })
                        .Add(_dirTileFactory.Pools,
                            new DirectionComponent(),
                            new DirectionTileComponent() { RuleName = "direction_triangle_rule" },
                            new DirectionBasedOnSpeed()
                        );

                    _lightSourceFactory.Pools.Pool1.Add(ent) = new LightSourceComponent()
                    {
                        Radius = 16,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.None,
                            Value = 32
                        }
                    };

                    break;

                case EntityType.Enemy:
                    //Debug.Assert(_enemyFactory.IsBelongToWorld(world));

                    ent.Add(_enemyFactory.Pools,
                            new RandomGeneratorComponent { Rnd = rnd },
                            new SpeedComponent(),
                            new SymbolComponent
                            {
                                Value = '☺',
                                Depth = Depth.Foreground,
                                MainColor = ConsoleColor.Red
                            },
                            new MoveFrictionComponent { FrictionValue = 1 },
                            new WaitCommandTokenComponent(1))
                        .Add(_dirTileFactory.Pools,
                            new DirectionComponent(),
                            new DirectionTileComponent() { RuleName = "direction_v_rule" },
                            new DirectionBasedOnSpeed()
                        );
                    ;

                    //_lightSourceFactory.Pools.Pool1.Add(ent) = new LightSourceComponent()
                    //{
                    //    Radius = 16,
                    //    BasicParameters = new LightValueComponent()
                    //    {
                    //        LightType = LightType.Fire,
                    //        Value = 255
                    //    }
                    //};

                    break;

                case EntityType.Electricity:
                    //Debug.Assert(_lightSourceFactory.IsBelongToWorld(world));

                    _lightSourceFactory.Pools.Pool1.Add(ent) = new LightSourceComponent()
                    {
                        Radius = 3,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Electricity,
                            Value = 32
                        }
                    };
                    _wallFactory.Pools.Pool1.Add(ent) = new SymbolComponent('Ω');

                    break;
                case EntityType.Light:
                    //Debug.Assert(_lightSourceFactory.IsBelongToWorld(world));
                    _lightSourceFactory.Pools.Pool1.Set(ent, new LightSourceComponent()
                    {
                        Radius = 16,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Fire,
                            Value = 196
                        }
                    });
                    _wallFactory.Pools.Pool1.Add(ent) = new SymbolComponent('i');
                    break;
                case EntityType.Acid:
                    //Debug.Assert(_lightSourceFactory.IsBelongToWorld(world));

                    _lightSourceFactory.Pools.Pool1.Add(ent) = new LightSourceComponent()
                    {
                        Radius = 4,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Acid,
                            Value = 32
                        }
                    };
                    _wallFactory.Pools.Pool1.SetObsolete(ent) = new SymbolComponent('▒');

                    break;
            }
        }
    }
}