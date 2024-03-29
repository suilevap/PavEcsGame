﻿using System;
using System.IO;
using System.Linq;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsGame;

namespace PavEcsGame.Systems
{
    internal class LoadMapSystem : IEcsInitSystem, IEcsSystemSpec
    {
        private readonly string _fileName;
        private readonly IMapData<PositionComponent, EcsPackedEntityWithWorld> _map;
        private readonly EcsEntityFactorySpec<EcsSpec<MapLoadedEvent>> _mapChangedEventFactory;
        
        private readonly EcsEntityFactorySpec<
            EcsSpec<NewPositionComponent, SpawnRequestComponent>> _spawnSpec;

        public LoadMapSystem(string fileName, EcsUniverse universe, IMapData<PositionComponent, EcsPackedEntityWithWorld> map)
        {
            _fileName = fileName;
            _map = map;

            universe
                .Register(this)
                .Build(ref _spawnSpec)
                .Build(ref _mapChangedEventFactory);
        }

        public async void Init(IEcsSystems systems)
        {
            var lines = await File.ReadAllLinesAsync(_fileName);

            if (lines == null || lines.Length == 0)
                return;

            _map.Init(new PositionComponent(new Int2(lines.Max(x => x.Length), lines.Length)));

            _mapChangedEventFactory.NewUnsafeEntity()
                .Add(_mapChangedEventFactory.Pools,
                    new MapLoadedEvent()
                    {
                        Size = _map.MaxPos.Value - _map.MinPos.Value
                    });

            var pos = new Int2();
            foreach (var line in lines)
            {
                pos.X = 0;
                foreach (var c in line)
                {
                    if (TryGetSpawnRequest(c).TryGet(out var request))
                    {
                        var ent = _spawnSpec.NewUnsafeEntity()
                            .Add(
                                _spawnSpec.Pools,
                                new NewPositionComponent()
                                {
                                    Value = pos
                                },
                                request
                            );
                    }

                    pos.X++;
                }

                pos.Y++;
            }

        }

        private SpawnRequestComponent? TryGetSpawnRequest(char symbol)
        {
            SpawnRequestComponent? result = default;
            switch (symbol)
            {
                //wall
                case 'X':
                case 'x':
                    result = new SpawnRequestComponent()
                    {
                        Type = EntityType.Wall
                    };
                    break;
                //player
                case 'p':
                    result = new SpawnRequestComponent()
                    {
                        Type = EntityType.Player
                    };
                    break;
                //enemy
                case 'e':
                    result = new SpawnRequestComponent()
                    {
                        Type = EntityType.Enemy
                    };
                    break;

                case '~':
                    result = new SpawnRequestComponent()
                    {
                        Type = EntityType.Electricity
                    };
                    break;
                case 'i':
                    result = new SpawnRequestComponent()
                    {
                        Type = EntityType.Light
                    };
                    break;
                case '%':
                    result = new SpawnRequestComponent()
                    {
                        Type = EntityType.Acid
                    };
                    break;
            }

            return result;
        }
    }
}