using System.Diagnostics;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems;
using PavEcsSpec.EcsLite;
using PaveEcsGame;

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

        public static ref T Set<T>(this EcsPool<T> pool, EcsUnsafeEntity ent)
            where T : struct
        {
            if (pool.Has(ent))
                return ref pool.Get(ent);
            return ref pool.Add(ent);
        }

        public static ref T GetOrAdd<T>(this EcsPool<T> pool, EcsUnsafeEntity ent, out bool exists)
            where T : struct
        {
            if (pool.Has(ent))
            {
                exists = true;
                return ref pool.Get(ent);
            }
            exists = false;
            return ref pool.Add(ent);
        }
    }
}