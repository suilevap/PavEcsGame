using System.Collections.Generic;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Tiles;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class DirectionTileSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly Dictionary<string, DirectionTileRule> _rules = new();

        [Entity]
        private partial struct Entity
        {
            public partial ref readonly DirectionComponent Dir();
            public partial ref readonly DirectionTileComponent Tile();
            public partial ref SymbolComponent Symbol();
        }

        public void Init(IEcsSystems systems)
        {
            //preload
            TryGetRule("direction_arrow_rule");
            TryGetRule("direction_triangle_rule");
            TryGetRule("direction_v_rule");
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in _providers.EntityProvider)
            {
                var tileRule = TryGetRule(ent.Tile().RuleName);
                if (tileRule != null)
                {
                    var dir = ent.Dir().Direction.ToDirection();
                    ent.Symbol().Value = tileRule.GetSymbol(dir);
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