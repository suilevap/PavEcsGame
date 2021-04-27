using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;
using PavEcsGame.LeoEcsExtensions;
using PavEcsGame.Utils;

namespace PavEcsGame.Systems
{
    class LoadMapSystem : IEcsInitSystem
    {
        private readonly string _fileName;
        private EcsWorld _world = null;
        private IMapData<PositionComponent, EcsEntity> _map;

        private WorkQueue _work;

        public LoadMapSystem(string fileName)
        {
            _fileName = fileName;
        }

        public async void Init()
        {

            var lines = await File.ReadAllLinesAsync(_fileName);

            if (lines == null || lines.Length == 0)
                return;

            await _work.ContinueInRun();

            _map.Init(new PositionComponent(new Int2(lines.Max(x => x.Length), lines.Length)));

            PositionComponent pos = new PositionComponent();
            foreach (var line in lines)
            {
                pos.Value.X = 0;
                foreach (var c in line)
                {
                    TrySpawnEntity(pos, c);
                    pos.Value.X++;
                }
                pos.Value.Y++;
            }

        }

        private void TrySpawnEntity(PositionComponent pos, char symbol)
        {
            switch (symbol)
            {
                //wall
                case 'X':
                case 'x':
                    _world.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Replace(new NewPositionComponent() { Value = pos })
                        .Replace(new SymbolComponent() { Value = '#' });
                    break;
                //player
                case 'p':
                    _world.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Replace(new PlayerIndexComponent() { Index = 0})
                        .Replace(new SpeedComponent())
                        .Replace(new NewPositionComponent() { Value = pos })
                        .Replace(new SymbolComponent() { Value = '@' });
                    break;
                //enemy
                case 'e':
                    _world.NewEntity()
                        //.Tag<SpawnRequestComponent>()
                        .Replace(new RandomGeneratorComponent() { Rnd = new Random(pos.Value.GetHashCode())})
                        .Replace(new NewPositionComponent() { Value = pos })
                        .Replace(new SymbolComponent() { Value = 'e' })
                        .Replace(new SpeedComponent());
                    break;


            }
        }
    }
}
