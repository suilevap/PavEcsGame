using System;
using System.Linq;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class LoadMapSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly IMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        [Entity]
        private readonly partial struct MapDataEnt
        {
            public partial RequiredComponent<MapRawDataEvent> Event();
        }

        [Entity(SkipFilter = true)]
        private readonly partial struct MapChangedEventEnt
        {
            public partial ref MapLoadedEvent Event();
        }

        [Entity(SkipFilter = true)]
        private readonly partial struct SpawnEnt
        {
            public partial ref NewPositionComponent NewPos();
            public partial ref SpawnRequestComponent Request();

        }

        public LoadMapSystem( EcsSystems universe, IMapData<PositionComponent, EcsPackedEntityWithWorld> map)
            : this(universe)
        {
            _map = map;

        }

        public void Run(EcsSystems systems)
        {
            //var lines = await File.ReadAllLinesAsync(_fileName);

            //if (lines == null || lines.Length == 0)
            //    return;

            foreach (var ent in _providers.MapDataEntProvider)
            {
                var lines = ent.Event().Get().Data;
                BuildMap(lines);
                ent.Event().Remove();
            }

        }

        private void BuildMap(string[] lines)
        {
            _map.Init(new PositionComponent(new Int2(lines.Max(x => x.Length), lines.Length)));

            _providers.MapChangedEventEntProvider
                .New()
                .Event()
                .Size = _map.MaxPos.Value - _map.MinPos.Value;

            var pos = new PositionComponent();
            foreach (var line in lines)
            {
                pos.Value.X = 0;
                foreach (var c in line)
                {
                    if (TryGetSpawnRequest(c).TryGet(out var request))
                    {
                        var spawnEnt = _providers.SpawnEntProvider
                            .New();
                        spawnEnt.NewPos().Value = pos;
                        spawnEnt.Request() = request;
                    }

                    pos.Value.X++;
                }

                pos.Value.Y++;
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