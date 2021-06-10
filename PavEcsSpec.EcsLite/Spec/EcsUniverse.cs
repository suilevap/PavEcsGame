using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            VerifySystemsOrder(systems);
            _builder = null;
            foreach (var initSpec in _registeredSpec)
            {
                initSpec.Init(systems);
            }
            _registeredSpec.Clear();
        }


        private string GetDebugDebInfo(IEcsSystemSpec system)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(system);
            sb.Append(" ");

            var deps = GetComponentsPermissions(system);
            foreach (var gr in deps.GroupBy(x => x.permission))
            {
                sb.AppendFormat("{0}:({1})", gr.Key, string.Join(",", gr.Select(x => x.type.Name)));
            }
            return sb.ToString();
        }
        private IEnumerable<(Type type, SpecPermissions permission)> GetComponentsPermissions(IEcsSystemSpec system)
        {
            if (_builder == null)
                return Enumerable.Empty<(Type type, SpecPermissions permission)>();
            return _builder.GetComponentsPermissions(system);
        }

        private void VerifySystemsOrder(EcsSystems systems)
        {
            var allSystems = _builder.GetSystemsSorted(out var cycles);
            var result = string.Join(",", allSystems.Select(x => $"{x.Key}:{x.Value}"));

            Debug.WriteLine(result);
            Debug.WriteLine($"Potential max even length:{cycles}");

            //IEcsSystem[] registeredSystems = null;
            //var count = systems.GetAllSystems(ref registeredSystems);
            //for (int i = 0; i < count; i++)
            //{
            //    if (registeredSystems[i] is IEcsSystemSpec system)
            //    {

            //    }
            //}
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

        internal EcsFilterSpecBuilder<TIncl, TOptional, TExclude> CreateFilterSpec<TIncl, TOptional, TExclude>(
            IEcsSystemSpec system,
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
                EcsFilterSpecBuilder<TIncl, TOptional, TExclude>.Create(_builder, system, include, optional, exclude);
            _registeredSpec.Add(result);
            return result;
        }

        internal EcsEntityFactorySpecBuilder<TPools> CreateEntityFactorySpec<TPools>(
            IEcsSystemSpec system,
            IEcsSpecBuilder<TPools> pools
        )
            where TPools : struct
        {
            Debug.Assert(_builder != null, "Creation filter after init is not supported");

            var result =
                EcsEntityFactorySpecBuilder<TPools>.Create(_builder, system, pools);
            _registeredSpec.Add(result);
            return result;
        }

        internal EcsEntityFactorySpecBuilder<TPools> CreateEntityFactorySpec<TPools, TParentPools>(
            IEcsSystemSpec system,
            IEcsSpecBuilder<TPools> pools,
            IEcsSpecBuilder<TParentPools> parentPools
        )
            where TPools : struct
            where TParentPools : struct
        {
            Debug.Assert(_builder != null, "Creation filter after init is not supported");

            var result =
                EcsEntityFactorySpecBuilder<TPools>.Create(_builder, system, pools, parentPools);
            _registeredSpec.Add(result);
            return result;
        }

    }
}