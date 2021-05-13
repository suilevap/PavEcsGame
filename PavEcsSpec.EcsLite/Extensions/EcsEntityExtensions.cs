using System.Diagnostics;
using System.Linq;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public static class EcsEntityExtensions
    {
        public static EcsUnsafeEntity Add<T1>(
            this EcsUnsafeEntity entityId,
            EcsSpec<T1> spec,
            in T1 c1
        )
            where T1 : struct
        {
            spec.Pool1.Add(entityId) = c1;
            return entityId;
        }

        public static EcsUnsafeEntity Add<T1, T2>(
            this EcsUnsafeEntity entityId,
            EcsSpec<T1, T2> spec,
            in T1 c1,
            in T2 c2
        )
            where T1 : struct
            where T2 : struct
        {
            spec.Pool1.Add(entityId) = c1;
            spec.Pool2.Add(entityId) = c2;
            return entityId;
        }

        public static EcsUnsafeEntity Add<T1, T2, T3>(
            this EcsUnsafeEntity entityId,
            EcsSpec<T1, T2, T3> spec,
            in T1 c1,
            in T2 c2,
            in T3 c3
        )
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {
            spec.Pool1.Add(entityId) = c1;
            spec.Pool2.Add(entityId) = c2;
            spec.Pool3.Add(entityId) = c3;
            return entityId;
        }

        public static EcsUnsafeEntity Add<T1, T2, T3, T4>(
            this EcsUnsafeEntity entityId,
            EcsSpec<T1, T2, T3, T4> spec,
            in T1 c1,
            in T2 c2,
            in T3 c3,
            in T4 c4
        )
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
        {
            spec.Pool1.Add(entityId) = c1;
            spec.Pool2.Add(entityId) = c2;
            spec.Pool3.Add(entityId) = c3;
            spec.Pool4.Add(entityId) = c4;
            return entityId;
        }

        public static EcsUnsafeEntity Add<T1, T2, T3, T4, T5>(
            this EcsUnsafeEntity entityId,
            EcsSpec<T1, T2, T3, T4, T5> spec,
            in T1 c1,
            in T2 c2,
            in T3 c3,
            in T4 c4,
            in T5 c5
        )
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
        {
            spec.Pool1.Add(entityId) = c1;
            spec.Pool2.Add(entityId) = c2;
            spec.Pool3.Add(entityId) = c3;
            spec.Pool4.Add(entityId) = c4;
            spec.Pool5.Add(entityId) = c5;
            return entityId;
        }

        public static EcsUnsafeEntity Add<T1, T2, T3, T4, T5, T6>(
            this EcsUnsafeEntity entityId,
            EcsSpec<T1, T2, T3, T4, T5, T6> spec,
            in T1 c1,
            in T2 c2,
            in T3 c3,
            in T4 c4,
            in T5 c5,
            in T6 c6
        )
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
        {
            spec.Pool1.Add(entityId) = c1;
            spec.Pool2.Add(entityId) = c2;
            spec.Pool3.Add(entityId) = c3;
            spec.Pool4.Add(entityId) = c4;
            spec.Pool5.Add(entityId) = c5;
            spec.Pool6.Add(entityId) = c6;
            return entityId;
        }

        //public static EcsPackedEntityWithWorld? TryAdd<T1>(
        //    this EcsPackedEntityWithWorld? entity,
        //    EcsSpec<T1> spec,
        //    in T1 c1
        //)
        //    where T1 : struct
        //{
        //    EcsUnsafeEntity id;
        //    if (entity.TryGet(out var ent) &&
        //        ent.Unpack(out var world, out id))
        //    {
        //        spec.AssertIsBelongToWorld(world);

        //        id.Add(
        //            spec,
        //            in c1
        //        );
        //        return entity;
        //    }

        //    return null;
        //}


        //public static EcsPackedEntityWithWorld? TryAdd<T1, T2>(
        //    this EcsPackedEntityWithWorld? entity,
        //    EcsSpec<T1, T2> spec,
        //    in T1 c1,
        //    in T2 c2
        //)
        //    where T1 : struct
        //    where T2 : struct
        //{
        //    EcsUnsafeEntity id;
        //    if (entity.TryGet(out var ent) &&
        //        ent.Unpack(out var world, out id))
        //    {
        //        spec.AssertIsBelongToWorld(world);

        //        id.Add(
        //            spec,
        //            in c1,
        //            in c2
        //        );
        //        return entity;
        //    }

        //    return null;
        //}


        //public static EcsPackedEntityWithWorld? TryAdd<T1, T2, T3>(
        //    this EcsPackedEntityWithWorld? entity,
        //    EcsSpec<T1, T2, T3> spec,
        //    in T1 c1,
        //    in T2 c2,
        //    in T3 c3
        //)
        //    where T1 : struct
        //    where T2 : struct
        //    where T3 : struct
        //{
        //    EcsUnsafeEntity id;
        //    if (entity.TryGet(out var ent) &&
        //        ent.Unpack(out var world, out id))
        //    {
        //        spec.AssertIsBelongToWorld(world);

        //        id.Add(
        //            spec,
        //            in c1,
        //            in c2,
        //            in c3
        //        );
        //        return entity;
        //    }

        //    return null;
        //}

        //public static EcsPackedEntityWithWorld? TryAdd<T1, T2, T3, T4>(
        //    this EcsPackedEntityWithWorld? entity,
        //    EcsSpec<T1, T2, T3, T4> spec,
        //    in T1 c1,
        //    in T2 c2,
        //    in T3 c3,
        //    in T4 c4
        //)
        //    where T1 : struct
        //    where T2 : struct
        //    where T3 : struct
        //    where T4 : struct
        //{
        //    EcsUnsafeEntity id;
        //    if (entity.TryGet(out var ent) &&
        //        ent.Unpack(out var world, out id))
        //    {
        //        spec.AssertIsBelongToWorld(world);

        //        id.Add(
        //            spec,
        //            in c1,
        //            in c2,
        //            in c3,
        //            in c4
        //        );
        //        return entity;
        //    }

        //    return null;
        //}

        //public static EcsPackedEntityWithWorld? TryAdd<T1, T2, T3, T4, T5>(
        //    this EcsPackedEntityWithWorld? entity,
        //    EcsSpec<T1, T2, T3, T4, T5> spec,
        //    in T1 c1,
        //    in T2 c2,
        //    in T3 c3,
        //    in T4 c4,
        //    in T5 c5
        //)
        //    where T1 : struct
        //    where T2 : struct
        //    where T3 : struct
        //    where T4 : struct
        //    where T5 : struct
        //{
        //    EcsUnsafeEntity id;
        //    if (entity.TryGet(out var ent) &&
        //        ent.Unpack(out var world, out id))
        //    {
        //        spec.AssertIsBelongToWorld(world);

        //        id.Add(
        //            spec,
        //            in c1,
        //            in c2,
        //            in c3,
        //            in c4,
        //            in c5
        //        );
        //        return entity;
        //    }

        //    return null;
        //}

        //public static EcsPackedEntityWithWorld? TryAdd<T1, T2, T3, T4, T5, T6>(
        //    this EcsPackedEntityWithWorld? entity,
        //    EcsSpec<T1, T2, T3, T4, T5, T6> spec,
        //    in T1 c1,
        //    in T2 c2,
        //    in T3 c3,
        //    in T4 c4,
        //    in T5 c5,
        //    in T6 c6
        //)
        //    where T1 : struct
        //    where T2 : struct
        //    where T3 : struct
        //    where T4 : struct
        //    where T5 : struct
        //    where T6 : struct
        //{
        //    EcsUnsafeEntity id;
        //    if (entity.TryGet(out var ent) &&
        //        ent.Unpack(out var world, out id))
        //    {
        //        spec.AssertIsBelongToWorld(world);
        //        id.Add(
        //            spec,
        //            in c1,
        //            in c2,
        //            in c3,
        //            in c4,
        //            in c5,
        //            in c6
        //        );
        //        return entity;
        //    }

        //    return null;
        //}
        public static bool Unpack(this in EcsPackedEntityWithWorld entity, out EcsWorld world,
            out EcsUnsafeEntity unsafeEnt)
        {
            var result = entity.Unpack(out world, out int id);
            unsafeEnt = new EcsUnsafeEntity(id);
            return result;
        }

        public static bool Unpack(this in EcsPackedEntityWithWorld? entity, out EcsWorld world,
            out EcsUnsafeEntity unsafeEnt)
        {
            if (entity.TryGet(out var ent))
            {
                return ent.Unpack(out world, out unsafeEnt);
            }
            else
            {
                world = null;
                unsafeEnt = default;
                return false;
            }
        }

        public static bool IsSame(this EcsPackedEntityWithWorld ent, EcsUnsafeEntity unsafeId)
        {
            return ent.Unpack(out _, out EcsUnsafeEntity id) && id == unsafeId;
        }
        public static bool IsBelongTo(this EcsPackedEntityWithWorld ent, IEcsLinkedToWorld worldContainer)
        {
            return ent.Unpack(out var world, out EcsUnsafeEntity _) 
                   && worldContainer.IsBelongToWorld(world);
        }


        public static string ToLogString(this EcsPackedEntityWithWorld ent)
        {
            if (ent.Unpack(out var world, out var id))
            {
                object[] components = null;

                world.GetComponents(id,  ref components);
                return $"Ent:{id} ->" + string.Join("|", components.Where(x=>x!= null).Select(x => x.ToString()));
            }
            return "Ent: -destroyed-";
        }


        public static void AssertIsNotEmpty(this EcsPackedEntityWithWorld? ent)
        {
            Debug.Assert(ent.HasValue, "Entity is expected here");
        }

        public static void AssertIsBelongToWorld<T>(this T spec, EcsWorld world)
            where T : struct, IEcsLinkedToWorld
        {
            Debug.Assert(spec.IsBelongToWorld(world), $"Spec: {spec} doesn't belong to the world: {world}");
        }
    }
}