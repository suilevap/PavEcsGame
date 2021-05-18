using System;
using System.IO;
using System.Linq;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PaveEcsGame;

namespace PavEcsGame.Systems
{
    internal class LoadMapSystem : IEcsInitSystem
    {
        private readonly EcsEntityFactorySpec<
                EcsSpec<NewPositionComponent, IsActiveTag>> _commonFactory;

        private readonly string _fileName;
        private readonly IMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

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
        private readonly EcsEntityFactorySpec<EcsSpec<MapLoadedEvent>> _mapChangedEventFactory;
        private readonly EcsEntityFactorySpec<EcsSpec<LightSourceComponent, PositionComponent>> _lightSourceFactory;
        private readonly EcsEntityFactorySpec<EcsSpec<VisualSensorComponent>> _actorFactory;


        public LoadMapSystem(string fileName, EcsUniverse universe, IMapData<PositionComponent, EcsPackedEntityWithWorld> map)
        {
            _fileName = fileName;
            _map = map;

            universe
                .Build(ref _commonFactory)
                .Build(_commonFactory, ref _actorFactory)
                .Build(_actorFactory, ref _playerFactory)
                .Build(_commonFactory, ref _lightSourceFactory)
                .Build(_actorFactory, ref _enemyFactory)
                .Build(_commonFactory, ref _wallFactory)
                .Build(ref _mapChangedEventFactory);
        }

        public async void Init(EcsSystems systems)
        {

            var lines = await File.ReadAllLinesAsync(_fileName);

            if (lines == null || lines.Length == 0)
                return;

            var rnd = new Random(42);

            _map.Init(new PositionComponent(new Int2(lines.Max(x => x.Length), lines.Length)));

            _mapChangedEventFactory.NewUnsafeEntity()
                .Add(_mapChangedEventFactory.Pools,
                    new MapLoadedEvent()
                    {
                        Size = _map.MaxPos.Value - _map.MinPos.Value
                    });

            var pos = new PositionComponent();
            foreach (var line in lines)
            {
                pos.Value.X = 0;
                foreach (var c in line)
                {
                    var ent = TrySpawnEntity(c, rnd);
                    if (ent.Unpack(out _, out var unsafeEnt))
                    {
                        unsafeEnt.Add(_commonFactory.Pools,
                            new NewPositionComponent {Value = pos},
                            default
                        );
                    }

                    pos.Value.X++;
                }

                pos.Value.Y++;
            }

        }

        private EcsPackedEntityWithWorld? TrySpawnEntity(char symbol, Random rnd)
        {
            EcsPackedEntityWithWorld? result = default;
            EcsUnsafeEntity ent;
            switch (symbol)
            {
                //wall
                case 'X':
                case 'x':
                    ent = _wallFactory.NewUnsafeEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(_wallFactory.Pools, 
                            new SymbolComponent
                            {
                                Value = '#', 
                                Depth = Depth.Foreground,
                                MainColor = ConsoleColor.Gray
                            },
                            new TileComponent(){RuleName = "wall_rule"});
                    result = _wallFactory.World.PackEntityWithWorld(ent);
                    break;
                //player
                case 'p':
                    ent = _playerFactory.NewUnsafeEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(_playerFactory.Pools,
                            new PlayerIndexComponent {Index = 0},
                            new SpeedComponent(),
                            new SymbolComponent
                            {
                                Value = '@', 
                                Depth = Depth.Foreground,
                                MainColor = ConsoleColor.White
                            },
                            new MoveFrictionComponent {FrictionValue = 1},
                            new WaitCommandTokenComponent(1))
                        .Add(_actorFactory.Pools,
                            new VisualSensorComponent(){Radius = 32});
                    

                    _lightSourceFactory.Pools.Pool1.SetObsolete(ent) = new LightSourceComponent()
                    {
                        Radius = 16, 
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Fire,
                            Value = 255
                        }
                    };

                    result = _playerFactory.World.PackEntityWithWorld(ent);
                    break;
                //enemy
                case 'e':
                    ent = _enemyFactory.NewUnsafeEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(_enemyFactory.Pools,
                            new RandomGeneratorComponent {Rnd = rnd},
                            new SpeedComponent(),
                            new SymbolComponent
                            {
                                Value = 'e', 
                                Depth = Depth.Foreground,
                                MainColor = ConsoleColor.Red
                            },
                            new MoveFrictionComponent {FrictionValue = 1},
                            new WaitCommandTokenComponent(1))
                        ;
                    //_lightSourceFactory.Pools.Pool1.Set(ent) = new LightSourceComponent(){ Radius = 8};

                    //_lightSourceFactory.Pools.Pool1.Set(ent) = new LightSourceComponent()
                    //{
                    //    Radius = 8,
                    //    Color = System.Drawing.Color.Blue.ToColor()
                    //};
                    result = _enemyFactory.World.PackEntityWithWorld(ent);
                    break;

                case '~':
                    ent = _lightSourceFactory.NewUnsafeEntity();

                    _lightSourceFactory.Pools.Pool1.SetObsolete(ent) = new LightSourceComponent()
                    {
                        Radius = 3,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Electricity,
                            Value = 32
                        }
                    };
                    _wallFactory.Pools.Pool1.SetObsolete(ent) = new SymbolComponent('Ω');

                    result = _playerFactory.World.PackEntityWithWorld(ent);
                    break;
                //case 'i':
                //    ent = _lightSourceFactory.NewUnsafeEntity();

                //    _lightSourceFactory.Pools.Pool1.Set(ent) = new LightSourceComponent()
                //    {
                //        Radius = 8,
                //        BasicParameters = new LightValueComponent()
                //        {
                //            LightType = LightType.Fire,
                //            Value = 16
                //        }
                //    };

                //    result = _playerFactory.World.PackEntityWithWorld(ent);
                //    break;
                case '%':
                    ent = _lightSourceFactory.NewUnsafeEntity();

                    _lightSourceFactory.Pools.Pool1.SetObsolete(ent) = new LightSourceComponent()
                    {
                        Radius = 4,
                        BasicParameters = new LightValueComponent()
                        {
                            LightType = LightType.Acid,
                            Value = 32
                        }
                    };
                    _wallFactory.Pools.Pool1.SetObsolete(ent) = new SymbolComponent('▒');

                    result = _playerFactory.World.PackEntityWithWorld(ent);
                    break;
            }

            return result;
        }
    }
}