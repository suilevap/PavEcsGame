using System.Collections.Generic;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PaveEcsGame.Tiles;

namespace PavEcsGame.Systems
{
    class DirectionTileSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<DirectionComponent, DirectionTileComponent>, EcsSpec<SymbolComponent>> _spec;

        private readonly Dictionary<string, DirectionTileRule> _rules = new Dictionary<string, DirectionTileRule>();

        public DirectionTileSystem(EcsUniverse universe)
        {
            universe
                .Register(this)
                .Build(ref _spec);
        }

        public void Init(EcsSystems systems)
        {
            //preload
            TryGetRule("direction_arrow_rule");
            TryGetRule("direction_triangle_rule");
            TryGetRule("direction_v_rule");
        }

        public void Run(EcsSystems systems)
        {
            var (dirPool, tilePool) = _spec.IncludeReadonly;
            var symbolPool = _spec.Include.Pool1;
            foreach (EcsUnsafeEntity ent in _spec.Filter)
            {
                ref readonly var tile = ref tilePool.Get(ent);
                var dir = dirPool.Get(ent).Direction.ToDirection();

                ref var symbol = ref symbolPool.Get(ent);
                var tileRule = TryGetRule(tile.RuleName);
                if (tileRule != null)
                {
                    symbol.Value = tileRule.GetSymbol(dir);
                }
            }
        }

        private DirectionTileRule TryGetRule(string name)
        {
            if (!_rules.TryGetValue(name, out var result))
            {
                result = DirectionTileRule.Load(name);
                _rules.Add(name, result);
            }

            return result;
        }

    }
}
