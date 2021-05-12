using System;
using System.IO;
using System.Linq;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;

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
                        .Add(_wallFactory.Pools, new SymbolComponent {Value = '#'});
                    result = _wallFactory.World.PackEntityWithWorld(ent);
                    break;
                //player
                case 'p':
                    ent = _playerFactory.NewUnsafeEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(_playerFactory.Pools,
                            new PlayerIndexComponent {Index = 0},
                            new SpeedComponent(),
                            new SymbolComponent {Value = '@'},
                            new MoveFrictionComponent {FrictionValue = 1},
                            new WaitCommandTokenComponent(1));
                    result = _playerFactory.World.PackEntityWithWorld(ent);
                    break;
                //enemy
                case 'e':
                    ent = _enemyFactory.NewUnsafeEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(_enemyFactory.Pools,
                            new RandomGeneratorComponent {Rnd = rnd},
                            new SpeedComponent(),
                            new SymbolComponent {Value = 'e'},
                            new MoveFrictionComponent {FrictionValue = 1},
                            new WaitCommandTokenComponent(1))
                        ;
                    result = _enemyFactory.World.PackEntityWithWorld(ent);
                    break;
            }

            return result;
        }
    }
}