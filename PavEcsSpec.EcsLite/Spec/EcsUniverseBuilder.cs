using System;
using System.Collections.Generic;
using System.Linq;

namespace PavEcsSpec.EcsLite
{
    internal class EcsUniverseBuilder
    {
        private readonly QuickUnionFind<Type> _worldUnion = new QuickUnionFind<Type>();

        internal EcsUniverseBuilder(EcsUniverse universe)
        {
            Universe = universe;
        }

        public EcsUniverse Universe { get; }

        internal void RegisterSet(IEnumerable<Type> required, IEnumerable<Type> optional)
        {
            _worldUnion.Union(required.Concat(optional));
        }


        internal Dictionary<Type, int> GetMapping()
        {
            var result = new Dictionary<Type, int>(_worldUnion.Count);

            foreach (var (worldId, type) in _worldUnion.GetAll()) result[type] = worldId;

            return result;
        }
    }
}