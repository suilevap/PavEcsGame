using System;
using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public class EcsUniverse : IEcsInitSystem
    {
        private readonly string _prefix;
        private readonly QuickUnionFind<Type> _quickUnionFind = new QuickUnionFind<Type>();

        private readonly List<IInitSpec> _registeredSpec = new List<IInitSpec>();

        public EcsUniverse(string prefix = "world_")
        {
            _prefix = prefix;
        }

        public void Init(EcsSystems systems)
        {
            foreach (var initSpec in _registeredSpec)
            {
                initSpec.Init(systems);
            }
            _registeredSpec.Clear();
        }

        internal Builder StartSet()
        {
            return new Builder(this);
        }

        public int GetKey<T>()
        {
            return _quickUnionFind.Root(typeof(T));
        }

        internal EcsWorld GetWorld<T>(EcsSystems systems)
        {
            var key = GetKey<T>();
            var name = GetName(key);
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

        public IEnumerable<IGrouping<EcsWorld, Type>> GetAllWorlds(EcsSystems systems)
        {
            return _quickUnionFind
                .GetAll()
                .GroupBy(p => systems.GetWorld(GetName(p.key)), p => p.item);
        }

        private string GetName(int key)
        {
            return _prefix + key;
        }

        public EcsFilterSpec<TIncl, TOptional, TExclude> CreateFilterSpec<TIncl, TOptional, TExclude>(
            IEcsSpecBuilder<TIncl> include,
            IEcsSpecBuilder<TOptional> optional,
            IEcsSpecBuilder<TExclude> exclude
        )
            where TIncl : struct
            where TOptional : struct
            where TExclude : struct
        {
            var result =
                EcsFilterSpec<TIncl, TOptional, TExclude>.Create(this, include, optional, exclude);
            _registeredSpec.Add(result);
            return result;
        }

        public EcsEntityFactorySpec<TPools> CreateEntityFactorySpec<TPools>(
            IEcsSpecBuilder<TPools> pools
        )
            where TPools : struct
        {
            var result = 
                EcsEntityFactorySpec<TPools>.Create(this, pools);
            _registeredSpec.Add(result);
            return result;
        }

        public EcsEntityFactorySpec<TPools> CreateEntityFactorySpec<TPools, TParentPools>(
            EcsEntityFactorySpec<TParentPools> parent,
            IEcsSpecBuilder<TPools> pools
        )
            where TPools : struct
            where TParentPools : struct
        {
            var result =
                EcsEntityFactorySpec<TPools>.Create(this, pools, parent); ;
            _registeredSpec.Add(result);
            return result;
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