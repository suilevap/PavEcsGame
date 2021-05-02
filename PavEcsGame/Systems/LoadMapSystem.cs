using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;
using PavEcsGame.Extensions;
using PavEcsGame.Utils;

namespace PavEcsGame.Systems
{
    class LoadMapSystem : IEcsInitSystem
    {
        private readonly string _fileName;
        private EcsWorld _world = null;
        private IMapData<PositionComponent, EcsEntity> _map;


        public LoadMapSystem(string fileName)
        {
            _fileName = fileName;
        }

        public async void Init()
        {

            var lines = await File.ReadAllLinesAsync(_fileName);

            if (lines == null || lines.Length == 0)
                return;

            Random rnd = new Random(42);

            _map.Init(new PositionComponent(new Int2(lines.Max(x => x.Length), lines.Length)));

            PositionComponent pos = new PositionComponent();
            foreach (var line in lines)
            {
                pos.Value.X = 0;
                foreach (var c in line)
                {
                    TrySpawnEntity(c, rnd)
                        ?.Replace(new NewPositionComponent() { Value = pos })
                        .Tag<IsActiveTag>();

                    pos.Value.X++;
                }
                pos.Value.Y++;
            }

        }

        private EcsEntity? TrySpawnEntity(char symbol, Random rnd)
        {
            EcsEntity? result = default;
            switch (symbol)
            {
                //wall
                case 'X':
                case 'x':
                    result = _world.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Replace(new SymbolComponent() { Value = '#' });
                    break;
                //player
                case 'p':
                    result = _world.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Replace(new PlayerIndexComponent() { Index = 0})
                        .Replace(new SpeedComponent())
                        .Replace(new SymbolComponent() { Value = '@' });
                    break;
                //enemy
                case 'e':
                    result = _world.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Replace(new RandomGeneratorComponent() { Rnd = rnd})
                        .Replace(new SymbolComponent() { Value = 'e' })
                        .Replace(new SpeedComponent());
                    break;
            }
            return result;
        }
    }
}
