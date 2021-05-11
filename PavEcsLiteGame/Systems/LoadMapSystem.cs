using System;
using System.IO;
using System.Linq;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Extensions;

namespace PavEcsGame.Systems
{
    internal class LoadMapSystem : IEcsInitSystem
    {
        private readonly EcsEntityFactorySpec<EcsSpec<NewPositionComponent, IsActiveTag>>
            _commonFactory;

        private readonly string _fileName;
        private readonly IMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        private readonly EcsEntityFactorySpec<EcsSpec<RandomGeneratorComponent, SpeedComponent, SymbolComponent,
            MoveFrictionComponent, WaitCommandTokenComponent>> _enemyFactory;

        private readonly EcsEntityFactorySpec<EcsSpec<PlayerIndexComponent, SpeedComponent, SymbolComponent,
            MoveFrictionComponent, WaitCommandTokenComponent>> _playerFactory;

        private readonly EcsEntityFactorySpec<EcsSpec<SymbolComponent>> _wallFactory;


        public LoadMapSystem(string fileName, EcsUniverse universe, IMapData<PositionComponent, EcsPackedEntityWithWorld> map)
        {
            _fileName = fileName;
            _map = map;

            _commonFactory = universe.CreateEntityFactorySpec(
                EcsSpec<
                    NewPositionComponent,
                    IsActiveTag>.Build()
            );

            _playerFactory = universe.CreateEntityFactorySpec(
                _commonFactory,
                EcsSpec<
                    PlayerIndexComponent,
                    SpeedComponent,
                    SymbolComponent,
                    MoveFrictionComponent,
                    WaitCommandTokenComponent>.Build()
            );

            _enemyFactory = universe.CreateEntityFactorySpec(
                _commonFactory,
                EcsSpec<
                    RandomGeneratorComponent,
                    SpeedComponent,
                    SymbolComponent,
                    MoveFrictionComponent,
                    WaitCommandTokenComponent>.Build()
            );


            _wallFactory = universe.CreateEntityFactorySpec(
                _commonFactory,
                EcsSpec<
                    SymbolComponent>.Build()
            );
        }

        public async void Init(EcsSystems systems)
        {
            _playerFactory.Init(systems);
            _commonFactory.Init(systems);
            _enemyFactory.Init(systems);
            _wallFactory.Init(systems);

            var lines = await File.ReadAllLinesAsync(_fileName);

            if (lines == null || lines.Length == 0)
                return;

            var rnd = new Random(42);

            _map.Init(new PositionComponent(new Int2(lines.Max(x => x.Length), lines.Length)));

            var pos = new PositionComponent();
            foreach (var line in lines)
            {
                pos.Value.X = 0;
                foreach (var c in line)
                {
                    var ent = TrySpawnEntity(c, rnd);
                    ent.TryAdd(_commonFactory.Pools,
                        new NewPositionComponent {Value = pos},
                        default
                    );

                    pos.Value.X++;
                }

                pos.Value.Y++;
            }
        }

        private EcsPackedEntityWithWorld? TrySpawnEntity(char symbol, Random rnd)
        {
            EcsPackedEntityWithWorld? result = default;
            switch (symbol)
            {
                //wall
                case 'X':
                case 'x':
                    result = _wallFactory.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .TryAdd(_wallFactory.Pools, new SymbolComponent {Value = '#'});
                    break;
                //player
                case 'p':
                    result = _playerFactory.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .TryAdd(_playerFactory.Pools,
                            new PlayerIndexComponent {Index = 0},
                            new SpeedComponent(),
                            new SymbolComponent {Value = '@'},
                            new MoveFrictionComponent {FrictionValue = 1},
                            new WaitCommandTokenComponent(1));
                    result.AssertIsNotEmpty();
                    break;
                //enemy
                case 'e':
                    result = _enemyFactory.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .TryAdd(_enemyFactory.Pools,
                            new RandomGeneratorComponent {Rnd = rnd},
                            new SpeedComponent(),
                            new SymbolComponent {Value = 'e'},
                            new MoveFrictionComponent {FrictionValue = 1},
                            new WaitCommandTokenComponent(1))
                        ;
                    break;
            }

            return result;
        }
    }
}