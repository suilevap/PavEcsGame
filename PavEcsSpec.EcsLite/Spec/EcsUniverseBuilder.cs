using System;
using System.Collections.Generic;
using System.Linq;

namespace PavEcsSpec.EcsLite
{
    internal class EcsUniverseBuilder
    {
        private readonly QuickUnionFind<Type> _worldUnion = new QuickUnionFind<Type>();

        private readonly DependencyGraph<IEcsSystemSpec, Type> _deps = new EcsLite.DependencyGraph<IEcsSystemSpec, Type>();

        public EcsUniverse Universe { get; }

        internal EcsUniverseBuilder(EcsUniverse universe)
        {
            Universe = universe;
        }

        internal void RegisterSet(IEcsSystemSpec system, IEnumerable<(Type type, SpecPermissions permissions)> args)
        {
            var dependencies = args
                .Where(x => x.permissions.HasFlag(SpecPermissions.Read))
                .Select(x => x.type);
            _deps.AddIncommingConnections(system, dependencies);

            var provides = args
                .Where(x => x.permissions.HasFlag(SpecPermissions.Create))
                .Select(x => x.type);
            _deps.AddOutgoingConnections(system, provides);

            _worldUnion.Union(args.Select(x=>x.type));
        }

        internal Dictionary<Type, int> GetMapping()
        {
            Dictionary<Type, int> result = new Dictionary<Type, int>(_worldUnion.Count);

            foreach (var (worldId, type) in _worldUnion.GetAll())
            {
                result[type] = worldId;
            }

            return result;
        }

        internal IEnumerable<(Type type, SpecPermissions permission)> GetComponentsPermissions(IEcsSystemSpec system)
        {
            foreach (var t in _deps.GetIncommingConnections(system))
            {
                yield return (t, SpecPermissions.Read);
            }

            foreach (var t in _deps.GetOutgoingConnections(system))
            {
                yield return (t, SpecPermissions.Create);
            }
        }

        internal Dictionary<IEcsSystemSpec, int> GetSystemsSorted(out int cycles)
        {
            Dictionary<IEcsSystemSpec, HashSet<IEcsSystemSpec>> deps = _deps.GetAllDeps()
                .ToDictionary(x => x.key, x => new HashSet<IEcsSystemSpec>(x.deps));

            Dictionary<IEcsSystemSpec, int> result = new Dictionary<IEcsSystemSpec, int>(deps.Count);

            int index = 0;
            cycles = 0;
            while (deps.Any())
            {
                var systems = GetSystemsWithoutDependecies().ToArray();
                if (!systems.Any())
                {
                    systems = GetSystemsWithMinDependecies().Take(1).ToArray();
                    cycles++;
                }
                if (!systems.Any())
                {
                    foreach (var p in deps)
                    {
                        result.Add(p.Key, -1);
                    }
                    break;
                }
                foreach (var system in systems)
                {
                    result.Add(system, index);
                    Remove(system);
                }
                index++;
            }

            return result;

            IEnumerable<IEcsSystemSpec> GetSystemsWithoutDependecies()
            {
                return deps.Where(x => !x.Value.Any())
                    .Select(x=>x.Key);
            }

            //IEnumerable<IEcsSystemSpec> GetSystemsWithMinDependecies()
            //{
            //    var minDepsCount = deps.Min(x => x.Value.Count);
            //    return deps.Where(x => x.Value.Count == minDepsCount)
            //        .Select(x => x.Key);
            //}

            IEnumerable<IEcsSystemSpec> GetSystemsWithMinDependecies()
            {
                Dictionary<IEcsSystemSpec, int> blockOthers = new Dictionary<IEcsSystemSpec, int>();
                foreach(var system in deps)
                {
                    foreach(var d in system.Value)
                    {
                        blockOthers.TryGetValue(d, out var count);
                        count++;
                        blockOthers[d] = count;
                    }
                }
                var maxCount = blockOthers.Max(x => x.Value);

                return blockOthers.Where(x => x.Value == maxCount)
                    .Select(x => x.Key);
            }

            void Remove(IEcsSystemSpec system)
            {
                deps.Remove(system);
                foreach (var pair in deps)
                {
                    pair.Value.Remove(system);
                }
            }
        }
    }
}