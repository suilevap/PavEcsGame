using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;
using PavEcsGame.Extensions;
using PavEcsGame.Utils;

namespace PavEcsGame.Systems
{
    class LoadMapSystem : IEcsInitSystem
    {
        private readonly string _fileName;
        private readonly IMapData<PositionComponent, EcsPackedEntity> _map;
        private readonly EcsEntityFactorySpec<EcsSpec<PositionComponent, NewPositionComponent, IsActiveTag>> _commonFactory;
        private EcsEntityFactorySpec<EcsSpec<PlayerIndexComponent, SpeedComponent, SymbolComponent, MoveFrictionComponent, WaitCommandTokenComponent>> _playerFactory;
        private EcsEntityFactorySpec<EcsSpec<RandomGeneratorComponent, SpeedComponent, SymbolComponent, MoveFrictionComponent, WaitCommandTokenComponent>> _enemyFactory;
        private EcsEntityFactorySpec<EcsSpec<SymbolComponent>> _wallFactory;


        public LoadMapSystem(string fileName, EcsUniverse universe, IMapData<PositionComponent, EcsPackedEntity> map)
        {
            _fileName = fileName;
            _map = map;

            _commonFactory = universe.CreateEntityFactorySpec(
                EcsSpec<
                    PositionComponent,
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


            //.Add(new RandomGeneratorComponent() { Rnd = rnd })
            //.Add(new SymbolComponent() { Value = 'e' })
            //.Add(new SpeedComponent())
            //.Add(new MoveFrictionComponent() { FrictionValue = 1 })
            //.Add(new WaitCommandTokenComponent(1));
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

            //var posPool = _world.GetPool<NewPositionComponent>();
            //var isActive = _world.GetPool<IsActiveTag>();
            Random rnd = new Random(42);

            _map.Init(new PositionComponent(new Int2(lines.Max(x => x.Length), lines.Length)));

            PositionComponent pos = new PositionComponent();
            foreach (var line in lines)
            {
                pos.Value.X = 0;
                foreach (var c in line)
                {
                    var possibleBuilder = TrySpawnEntity(c, rnd);
                    if (possibleBuilder.TryGet(out var builder))
                    {
                        builder
                            .Add(_commonFactory.Pools.Pool2, new NewPositionComponent() { Value = pos })
                            //TODO: remove after implementing UpdatePosSystem
                            .Add(_commonFactory.Pools.Pool1, pos)
                            .Tag<IsActiveTag>(_commonFactory.Pools.Pool3);
                    }

                    pos.Value.X++;
                }
                pos.Value.Y++;
            }

        }

        private EcsEntityBuilder? TrySpawnEntity(char symbol, Random rnd)
        {
            EcsEntityBuilder? result = default;
            switch (symbol)
            {
                //wall
                case 'X':
                case 'x':
                    result = _commonFactory.World.BuildNewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(_wallFactory.Pools.Pool1, new SymbolComponent() { Value = '#' });
                    break;
                //player
                case 'p':
                    result = _commonFactory.World.BuildNewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(_playerFactory.Pools.Pool1, new PlayerIndexComponent() { Index = 0 })
                        .Add(_playerFactory.Pools.Pool2, new SpeedComponent())
                        .Add(_playerFactory.Pools.Pool3, new SymbolComponent() { Value = '@' })
                        .Add(_playerFactory.Pools.Pool4, new MoveFrictionComponent() { FrictionValue = 1 })
                        .Add(_playerFactory.Pools.Pool5, new WaitCommandTokenComponent(1));

                    break;
                //enemy
                case 'e':
                    result = _commonFactory.World.BuildNewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(_enemyFactory.Pools.Pool1, new RandomGeneratorComponent() { Rnd = rnd })
                        .Add(_enemyFactory.Pools.Pool2, new SpeedComponent())
                        .Add(_enemyFactory.Pools.Pool3, new SymbolComponent() { Value = 'e' })
                        .Add(_enemyFactory.Pools.Pool4, new MoveFrictionComponent() { FrictionValue = 1 })
                        .Add(_enemyFactory.Pools.Pool5, new WaitCommandTokenComponent(1));
                    break;
            }
            return result;
        }
    }
}
