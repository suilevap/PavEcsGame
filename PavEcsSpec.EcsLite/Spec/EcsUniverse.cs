using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public class EcsUniverse : IEcsInitSystem
    {
        private readonly string _prefix;
        private readonly List<IInitSpec> _registeredSpec = new List<IInitSpec>();
        private EcsUniverseBuilder _builder;
        private Dictionary<Type, int> _requiredTypeToWorldId;

        public EcsUniverse(string prefix = "world_")
        {
            _prefix = prefix;
            _builder = new EcsUniverseBuilder(this);
        }

        public void Init(EcsSystems systems)
        {
            _requiredTypeToWorldId = _builder.GetMapping();
            _builder = null;
            foreach (var initSpec in _registeredSpec)
            {
                initSpec.Init(systems);
            }
            _registeredSpec.Clear();
        }

        private int GetKey<T>()
        {
            return _requiredTypeToWorldId[typeof(T)];
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
            return _requiredTypeToWorldId.Values;
            //return _worldUnion.GetAllRoots();
        }

        public IEnumerable<IGrouping<EcsWorld, Type>> GetAllWorlds(EcsSystems systems)
        {
            return _requiredTypeToWorldId
                .GroupBy(
                    p => systems.GetWorld(GetName(p.Value)), 
                    p => p.Key);
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
            Debug.Assert(_builder != null, "Creation filter after init is not supported");
            var result =
                EcsFilterSpec<TIncl, TOptional, TExclude>.Create(_builder, include, optional, exclude);
            _registeredSpec.Add(result);
            return result;
        }

        public EcsEntityFactorySpec<TPools> CreateEntityFactorySpec<TPools>(
            IEcsSpecBuilder<TPools> pools
        )
            where TPools : struct
        {
            Debug.Assert(_builder != null, "Creation filter after init is not supported");

            var result = 
                EcsEntityFactorySpec<TPools>.Create(_builder, pools);
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
            Debug.Assert(_builder != null, "Creation filter after init is not supported");

            var result =
                EcsEntityFactorySpec<TPools>.Create(_builder, pools, parent); ;
            _registeredSpec.Add(result);
            return result;
        }

    }
}