using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsGame.Tiles;
using PavEcsSpec.Generated;


namespace PavEcsGame.Systems
{
    partial class TileSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly IReadOnlyMapData<Int2, EcsPackedEntityWithWorld> _map;

        [Entity]
        private readonly partial struct Ent
        {
            public partial ref readonly PositionComponent Pos();
            public partial RequiredComponent<TileComponent> Tile();
            public partial ref SymbolComponent View();

        }

        private readonly Dictionary<string, TileRule> _rules = new Dictionary<string, TileRule>();

        public TileSystem(EcsSystems universe, IReadOnlyMapData<Int2, EcsPackedEntityWithWorld> map)
            : this(universe)
        {
            _map = map;
        }

        public void Init(EcsSystems systems)
        {
            //preload
            TryGetRule("wall_rule");
        }

        public void Run(EcsSystems systems)
        {
            foreach (Ent entity in _providers.EntProvider)
            {
                var tile = entity.Tile();
                ref var tileData = ref tile.Get();
                ref readonly var pos = ref entity.Pos();
                //entity.Id
                UpdateMask(ref tileData, pos.Value + new Int2(1, 0), 0);
                UpdateMask(ref tileData, pos.Value + new Int2(0, 1), 1);
                ref var view = ref entity.View();
                var tileRule = TryGetRule(tileData.RuleName);
                if (tileRule != null)
                {
                    view.Value = tileRule.Symbols[tileData.Mask];
                }
                tile.Remove();

            }

            void UpdateMask(ref TileComponent tile, in Int2 nextPos, int maskShift)
            {
                var nextId = _map.Get(_map.GetSafePos(nextPos));
                if (_providers.EntProvider.TryGet(nextId, out var nextEnt))
                {
                    ref var nextTile = ref nextEnt.Tile().Get();
                    if (nextTile.RuleName == tile.RuleName)
                    {
                        tile.Mask |= 1 << maskShift;
                        nextTile.Mask |= 1 << ((maskShift + 2) % 4);
                    }
                }
            }

        }

        private TileRule TryGetRule(string name)
        {
            if (!_rules.TryGetValue(name, out var result))
            {
                result = TileRule.Load(name);
                _rules.Add(name, result);
            }

            return result;
        }

    }
}
