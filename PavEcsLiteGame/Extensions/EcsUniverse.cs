﻿using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using PaveEcsGame.Utils;

namespace PavEcsGame.Extensions
{
    public class EcsUniverse
    {
        private readonly string _prefix;
        private readonly QuickUnionFind<Type> _quickUnionFind = new QuickUnionFind<Type>();

        public EcsUniverse(string prefix = "world_")
        {
            _prefix = prefix;
        }

        public Builder StartSet()
        {
            return new Builder(this);
        }

        public int GetKey<T>()
        {
            return _quickUnionFind.Root(typeof(T));
        }

        public EcsWorld GetWorld<T>(EcsSystems systems)
        {
            var key = GetKey<T>();
            var name = _prefix + key;
            var world = systems.GetWorld(name);
            if (world == null)
            {
                world = new EcsWorld();
                systems.AddWorld(world, name);
            }

            return world;
        }

        public IEnumerable<int> GetAllKeys()
        {
            return _quickUnionFind.GetAllRoots();
        }

        public class Builder
        {
            private readonly List<Type> _types = new List<Type>();
            private readonly EcsUniverse _universe;

            internal Builder(EcsUniverse universe)
            {
                _universe = universe;
            }

            public Builder Add<T>()
            {
                _types.Add(typeof(T));
                return this;
            }

            public void End()
            {
                _universe._quickUnionFind.Union(_types);
            }
        }
    }
}