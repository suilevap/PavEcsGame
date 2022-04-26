using System.Collections.Generic;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsGame.Tiles;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    partial class DirectionTileSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {

        [Entity]
        private partial struct Entity
        {
            public partial ref readonly DirectionComponent Dir();
            public partial ref readonly DirectionTileComponent Tile();
            public partial ref SymbolComponent Symbol();
        }

        private readonly Dictionary<string, DirectionTileRule> _rules = new Dictionary<string, DirectionTileRule>();

        public void Init(EcsSystems systems)
        {
            //preload
            TryGetRule("direction_arrow_rule");
            TryGetRule("direction_triangle_rule");
            TryGetRule("direction_v_rule");

        }

        public void Run(EcsSystems systems)
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
