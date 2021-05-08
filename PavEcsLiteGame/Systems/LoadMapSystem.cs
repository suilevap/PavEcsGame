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
        private readonly EcsWorld _world = null;
        private readonly IMapData<PositionComponent, EcsPackedEntity> _map;


        public LoadMapSystem(string fileName, EcsWorld world, IMapData<PositionComponent, EcsPackedEntity> map)
        {
            _fileName = fileName;
            _map = map;
            _world = world;
        }

        public async void Init(EcsSystems systems)
        {

            var lines = await File.ReadAllLinesAsync(_fileName);

            if (lines == null || lines.Length == 0)
                return;

            var posPool = _world.GetPool<NewPositionComponent>();
            var isActive = _world.GetPool<IsActiveTag>();
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
                            .Add(posPool, new NewPositionComponent() {Value = pos})
                            //TODO: remove after implementing UpdatePosSystem
                            .Add(pos)
                            .Tag<IsActiveTag>();
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
                    result = _world.BuildNewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(new SymbolComponent() { Value = '#' });
                    break;
                //player
                case 'p':
                    result = _world.BuildNewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(new PlayerIndexComponent() { Index = 0})
                        .Add(new SpeedComponent())
                        .Add(new SymbolComponent() { Value = '@' })
                        .Add(new MoveFrictionComponent() { FrictionValue = 1 })
                        .Add(new WaitCommandTokenComponent(1));

                    break;
                //enemy
                case 'e':
                    result = _world.BuildNewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Add(new RandomGeneratorComponent() { Rnd = rnd})
                        .Add(new SymbolComponent() { Value = 'e' })
                        .Add(new SpeedComponent())
                        .Add(new MoveFrictionComponent() { FrictionValue = 1 })
                        .Add(new WaitCommandTokenComponent(1));
                    break;
            }
            return result;
        }
    }
}
