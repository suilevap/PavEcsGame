using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;


namespace PavEcsGame.Systems
{
    internal partial class SpawnEntitySystem : IEcsRunSystem, IEcsSystemSpec
    {

        [Entity(SkipFilter = true)]
        private readonly partial struct CommonEntity
        {
            public partial ref IsActiveTag IsActive();

        }


        [Entity(SkipFilter = true)]
        private readonly partial struct LinkEntity
        {
            public partial CommonEntity Common();

            public partial ref LinkToEntityComponent<EcsEntity> LinkTo();
            public partial ref RelativePositionComponent RelPos();

        }


        [Entity(SkipFilter = true)]
        private readonly partial struct PhysicEntity
        {
            public partial CommonEntity Common();

            public partial ref ColliderComponent Collider();

        }

        [Entity(SkipFilter = true)]
        private readonly partial struct EnemyEntity
        {
            public partial ActorEntity Actor();

            public partial DirTileEntity DirTile();

            public partial ref RandomGeneratorComponent Rnd();
            public partial ref SpeedComponent Speed();
            public partial ref SymbolComponent View();
            public partial ref MoveFrictionComponent Friction();
            public partial ref WaitCommandTokenComponent WaitCommandToken();

        }

        [Entity(SkipFilter = true)]
        private readonly partial struct PlayerEntity
        {
            public partial ActorEntity Actor();

            public partial DirTileEntity DirTile();

            public partial LightEntity Light();

            public partial ref PlayerIndexComponent Index();
            public partial ref SpeedComponent Speed();
            public partial ref SymbolComponent View();
            public partial ref MoveFrictionComponent Friction();
            public partial ref WaitCommandTokenComponent WaitCommandToken();

        }

        [Entity(SkipFilter = true)]
        private readonly partial struct WallEntity
        {
            public partial PhysicEntity Physic();

            public partial ref SymbolComponent View();
            public partial ref TileComponent Tile();
        }

        [Entity(SkipFilter = true)]
        private readonly partial struct LightEntity
        {
            public partial CommonEntity Common();

            public partial OptionalComponent<PositionComponent> Pos();
            public partial ref LightSourceComponent Source();

            public partial OptionalComponent<SymbolComponent> View();

        }

        [Entity(SkipFilter = true)]
        private readonly partial struct ActorEntity
        {
            public partial PhysicEntity Physic();

            public partial ref VisualSensorComponent Sensor();
        }

        [Entity(SkipFilter = true)]
        private readonly partial struct DirTileEntity
        {
            public partial CommonEntity Common();

            public partial ref DirectionComponent Dir();
            public partial OptionalComponent<DirectionTileComponent> DirTile();

            public partial ref DirectionBasedOnSpeed DirBasedOnSpeed();
        }

        [Entity(SkipFilter = false)]
        private readonly partial struct SpawnRequestEntity
        {
            public partial RequiredComponent<SpawnRequestComponent> Request();
        }

        private readonly Random _rnd;

        public SpawnEntitySystem(EcsSystems universe)
        {
            _rnd = new Random(42);

            var commonProvider = CommonEntity.Create(universe);
            var dirTileProvider = DirTileEntity.Create(universe, commonProvider);

            var linkProvider = LinkEntity.Create(universe, commonProvider);
            var physicProvider = PhysicEntity.Create(universe, commonProvider);
            var actorProvider = ActorEntity.Create(universe, physicProvider);
            var lightProvider = LightEntity.Create(universe, commonProvider);

            var playerProvider = PlayerEntity.Create(universe, actorProvider, dirTileProvider, lightProvider);
            var enemyProvider = EnemyEntity.Create(universe, actorProvider, dirTileProvider);
            var wallProvider = WallEntity.Create(universe, physicProvider);
            ;
            _providers = new Providers(
                commonProvider,
                linkProvider,
                physicProvider,
                enemyProvider,
                playerProvider,
                wallProvider,
                lightProvider,
                actorProvider,
                dirTileProvider,
                SpawnRequestEntity.Create(universe)
                );
        }

        public void Run(EcsSystems systems)
        {
            foreach (var spawn in _providers.SpawnRequestEntityProvider)
            {
                ref var spawnReq = ref spawn.Request().Get();
                TrySpawnEntity((EcsUnsafeEntity)spawn.GetRawId(), spawnReq, _rnd);
                _providers.CommonEntityProvider.Ensure(spawn.GetRawId());
                spawn.Request().Remove();
            }
        }

        private void TrySpawnEntity(EcsUnsafeEntity ent, in SpawnRequestComponent request, Random rnd)
        {

            switch (request.Type)
            {
                case EntityType.Wall:
                    var wall = _providers.WallEntityProvider.Add(ent);
                    wall.View() = new SymbolComponent
                    {
                        Value = '#',
                        Depth = Depth.Foreground,
                        MainColor = ConsoleColor.Gray
                    };
                    wall.Tile() = new TileComponent() { RuleName = "wall_rule" };
                    break;
                case EntityType.Player:
                    //Debug.Assert(_playerFactory.IsBelongToWorld(world));

                    var player = _providers.PlayerEntityProvider.Add(ent);
                    player.Index() = new PlayerIndexComponent { Index = 0 };
                    player.View() = new SymbolComponent
                    {
                        Value = '@',
                        Depth = Depth.Foreground,
                        MainColor = ConsoleColor.White
                    };
                    player.Friction() = new MoveFrictionComponent { FrictionValue = 1 };
                    player.WaitCommandToken() = new WaitCommandTokenComponent(1);
                    player.Actor().Sensor() = new VisualSensorComponent() { Radius = 16 };

                    player.Light().Source() = new LightSourceComponent()
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
                    var enemy = _providers.EnemyEntityProvider.Add(ent);
                    enemy.Rnd().Rnd = rnd;
                    enemy.View() = new SymbolComponent
                    {
                        Value = '☺',
                        Depth = Depth.Foreground,
                        MainColor = ConsoleColor.Red
                    };
                    enemy.Friction() = new MoveFrictionComponent { FrictionValue = 1 };
                    enemy.WaitCommandToken() = new WaitCommandTokenComponent(1);
                    enemy.DirTile().DirTile().Ensure().RuleName = "direction_v_rule";

                    break;

                case EntityType.Electricity:
                    var el = _providers.LightEntityProvider.Add(ent);
                    el.Source() = new LightSourceComponent()
                    {
                        Radius = 4,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Electricity,
                            Value = 32
                        }
                    };
                    el.View().Ensure().Value = 'Ω';

                    break;
                case EntityType.Light:
                    //Debug.Assert(_lightSourceFactory.IsBelongToWorld(world));
                    var light = _providers.LightEntityProvider.Add(ent);
                    light.Source() = new LightSourceComponent()
                    {
                        Radius = 16,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Fire,
                            Value = 196
                        }
                    };
                    light.View().Ensure().Value = 'i';
                    break;
                case EntityType.Acid:
                    var acid = _providers.LightEntityProvider.Add(ent);
                    acid.Source() = new LightSourceComponent()
                    {
                        Radius = 4,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Acid,
                            Value = 32
                        }
                    };
                    acid.View().Ensure().Value = '▒';
                    break;
            }
        }
    }
}