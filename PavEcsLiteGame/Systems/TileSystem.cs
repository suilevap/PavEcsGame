using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PaveEcsGame.Tiles;

namespace PavEcsGame.Systems
{
    class TileSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly IReadOnlyMapData<Int2, EcsPackedEntityWithWorld> _map;
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<PositionComponent>, EcsSpec<TileComponent, SymbolComponent>> _spec;
        
        private readonly Dictionary<string, TileRule> _rules = new Dictionary<string, TileRule>();

        public TileSystem(EcsUniverse universe, IReadOnlyMapData<Int2, EcsPackedEntityWithWorld> map)
        {
            _map = map;
            universe
                .Register(this)
                .Build(ref _spec);
        }

        public void Init(EcsSystems systems)
        {
            //preload
            TryGetRule("wall_rule");
        }

        public void Run(EcsSystems systems)
        {
            var posPool = _spec.IncludeReadonly.Pool1;
            var (tilePool, symbolPool) = _spec.Include;
            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                ref var tile = ref tilePool.Get(ent);
                ref readonly var pos = ref posPool.Get(ent);

                UpdateMask(ent, ref tile, pos.Value + new Int2(1, 0), 0);
                UpdateMask(ent, ref tile, pos.Value + new Int2(0, 1), 1);

                ref var symbol = ref symbolPool.Get(ent);
                var tileRule = TryGetRule(tile.RuleName);
                if (tileRule != null)
                {
                    symbol.Value = tileRule.Symbols[tile.Mask];
                }

                tilePool.Del(ent);
            }

            //static int GetRule(Int2 pos)
            //{

            //}

            //static bool IsOccupied(in Int2 pos, TileRule)
            //{

            //}
            void UpdateMask(EcsUnsafeEntity ent, ref TileComponent tile, in Int2 nextPos, int maskShift)
            {
                var nextEnt = _map.Get(_map.GetSafePos(nextPos));
                if (nextEnt.Unpack(out _, out EcsUnsafeEntity nextId) &&
                    tilePool.Has(nextId))
                {
                    ref var nextTile = ref tilePool.Get(nextId);
                    if (nextTile.RuleName == tile.RuleName)
                    {
                        tile.Mask |= 1 << maskShift;
                        nextTile.Mask |= 1 << ((maskShift + 2)%4);
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
