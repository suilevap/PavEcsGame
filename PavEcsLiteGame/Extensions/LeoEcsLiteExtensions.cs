using System.Diagnostics;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using PavEcsSpec.Generated;

namespace PavEcsGame
{
    public static class LeoEcsLiteExtensions
    {

        public static EcsSystems SetSyncContext(this EcsSystems system)
        {
            return system.Add(new SynchronizationContextSystem());
        }

        public static EcsPackedEntityWithWorld? TryTag<T>(this EcsPackedEntityWithWorld entity, EcsPool<T> pool)
            where T : struct, ITag
        {
            EcsPackedEntityWithWorld? ent = entity;
            return ent.TryTag(pool, true);
        }

        public static EcsPackedEntityWithWorld? TryTag<T>(this EcsPackedEntityWithWorld entity, EcsPool<T> pool,
            bool value) where T : struct, ITag
        {
            EcsPackedEntityWithWorld? ent = entity;
            return ent.TryTag(pool, value);
        }

        public static EcsPackedEntityWithWorld? TryTag<T>(this EcsPackedEntityWithWorld? entity, EcsPool<T> pool)
            where T : struct, ITag
        {
            return entity.TryTag(pool, true);
        }

        public static EcsPackedEntityWithWorld? TryTag<T>(this EcsPackedEntityWithWorld? entity, EcsPool<T> pool,
            bool value) where T : struct, ITag
        {
            if (entity.TryGet(out var ent) &&
                ent.Unpack(out var world, out EcsUnsafeEntity id))
            {
                id.TryTag(pool, value);
                return entity;
            }

            return default;
        }

        public static EcsUnsafeEntity TryTag<T>(this EcsUnsafeEntity id, EcsPool<T> pool, bool value)
            where T : struct, ITag
        {
            if (pool.Has(id))
            {
                if (!value) pool.Del(id);
            }
            else
            {
                if (value) pool.Add(id);
            }

            return id;
        }

        public static void TryTag<T>(this PavEcsSpec.Generated.OptionalComponent<T> c, bool value)
            where T : struct, ITag
        {
            if (value)
            {
                c.Ensure();
            }
            else
            {
                c.Clear();
            }
        }

        public static bool IsEmpty(this EcsFilter filter)
        {
            return filter.GetEntitiesCount() == 0;
        }

        public static bool IsAlive(this in EcsPackedEntity packed, EcsWorld world)
        {
            return packed.Unpack(world, out _);
        }

        public static bool IsAlive(this in EcsPackedEntityWithWorld packed)
        {
            return packed.Unpack(out _, out int _);
        }

        public static void Set<T>(this EcsPool<T> pool, EcsUnsafeEntity ent, in T component)
            where T : struct
        {
            if (pool.Has(ent))
            {
                pool.Get(ent) = component;
            }
            else
            {
                pool.Add(ent) = component;
            }
        }
        public static ref T SetObsolete<T>(this EcsPool<T> pool, EcsUnsafeEntity ent)
            where T : struct
        {
            if (pool.Has(ent))
            {
                return ref pool.Get(ent);
            }
            else
            {
                return ref pool.Add(ent);
            }
        }

        public static ref T Ensure<T>(this EcsPool<T> pool, EcsUnsafeEntity ent, out bool isNew)
            where T : struct
        {
            if (pool.Has(ent))
            {
                isNew = false;
                return ref pool.Get(ent);
            }
            isNew = true;
            return ref pool.Add(ent);
        }

        public static ref T Ensure<T>(this EcsPool<T> pool, EcsUnsafeEntity ent)
            where T : struct
        {
            if (pool.Has(ent))
            {
                return ref pool.Get(ent);
            }
            return ref pool.Add(ent);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Unpack(this in EcsEntity packedEntity, out EcsWorld world, out EcsUnsafeEntity entity)
        {
            var result = Leopotam.EcsLite.EcsEntityExtensions.Unpack(packedEntity, out world, out var entId);
            entity = (EcsUnsafeEntity)entId;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsEntity Pack(this in EcsUnsafeEntity entId, EcsWorld world)
        {
            return Leopotam.EcsLite.EcsEntityExtensions.PackEntityWithWorld(world, entId);
        }


        public static EcsSystems MyDelHere<T>(this EcsSystems systems)
     where T : struct
        {
            return systems.Add(new GeneratedDelHereSystem<T>(systems));
        }

    }


    partial class GeneratedDelHereSystem<T> : IEcsRunSystem//, IEcsSystemSpec
        where T : struct
    {
        //private readonly EcsFilterSpec<EcsSpec<T>, EcsSpec, EcsSpec> _spec;

        //public UniverseDelHereSystem(EcsUniverse universe)
        //{
        //    universe
        //        .Register(this)
        //        .Build(ref _spec);
        //}

        public GeneratedDelHereSystem(EcsSystems systems)
        {
            var worldName = TypeToWorldNameMap.GetWorldName<T>();
            _providers = new Providers(Ent.Create(worldName, systems));
        }

        [Entity]
        private readonly partial struct Ent
        {
            public partial RequiredComponent<T> ComponentToDel();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _providers.EntProvider)
            {
                entity.ComponentToDel().Remove();
            }
        }
    }

}