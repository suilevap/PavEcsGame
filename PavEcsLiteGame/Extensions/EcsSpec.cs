using System;
using Leopotam.EcsLite;

namespace PavEcsGame.Extensions
{
    public interface IFilterGenerator
    {
        EcsFilter.Mask Include(EcsWorld world);
        EcsFilter.Mask Exclude(EcsFilter.Mask mask);
    }

    public readonly struct EcsSpec : IFilterGenerator
    {
        public EcsFilter.Mask Include(EcsWorld world)
        {
            throw new InvalidOperationException("Empty spec is impossible to use as include filter");
        }

        public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
        {
            return mask;
        }

        public static readonly EcsSpec Empty = new EcsSpec();

        public static EcsSpec CreateEmpty(EcsWorld _)
        {
            return new EcsSpec();
        }
    }

    public readonly struct EcsSpec<T1> : IFilterGenerator
        where T1 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsSpec(EcsPool<T1> pool1)
        {
            Pool1 = pool1;
        }

        public EcsFilter.Mask Include(EcsWorld world)
        {
            return world.Filter<T1>();
        }

        public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
        {
            return mask.Exc<T1>();
        }

        public static EcsSpec<T1> Create(EcsWorld world)
        {
            return new EcsSpec<T1>(
                world.GetPool<T1>()
            );
        }
    }

    public readonly struct EcsSpec<T1, T2> : IFilterGenerator
        where T1 : struct
        where T2 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsPool<T2> Pool2 { get; }


        public EcsSpec(in EcsSpec<T1> EcsSpec, EcsPool<T2> pool2)
        {
            Pool1 = EcsSpec.Pool1;
            Pool2 = pool2;
        }

        public EcsSpec(
            EcsPool<T1> pool1,
            EcsPool<T2> pool2
        )
        {
            Pool1 = pool1;
            Pool2 = pool2;
        }

        public EcsFilter.Mask Include(EcsWorld world)
        {
            return world
                .Filter<T1>()
                .Inc<T2>();
        }

        public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
        {
            return mask
                .Exc<T1>()
                .Exc<T2>();
        }

        public static EcsSpec<T1, T2> Create(EcsWorld world)
        {
            return new EcsSpec<T1, T2>(
                world.GetPool<T1>(),
                world.GetPool<T2>()
            );
        }
    }

    public readonly struct EcsSpec<T1, T2, T3> : IFilterGenerator
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsPool<T2> Pool2 { get; }

        public EcsPool<T3> Pool3 { get; }


        public EcsSpec(in EcsSpec<T1, T2> EcsSpec, EcsPool<T3> pool3)
        {
            Pool1 = EcsSpec.Pool1;
            Pool2 = EcsSpec.Pool2;
            Pool3 = pool3;
        }

        public EcsSpec(
            EcsPool<T1> pool1,
            EcsPool<T2> pool2,
            EcsPool<T3> pool3
        )
        {
            Pool1 = pool1;
            Pool2 = pool2;
            Pool3 = pool3;
        }

        public EcsFilter.Mask Include(EcsWorld world)
        {
            return world
                .Filter<T1>()
                .Inc<T2>()
                .Inc<T3>();
        }

        public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
        {
            return mask
                .Exc<T1>()
                .Exc<T2>()
                .Exc<T3>();
        }

        public static EcsSpec<T1, T2, T3> Create(EcsWorld world)
        {
            return new EcsSpec<T1, T2, T3>(
                world.GetPool<T1>(),
                world.GetPool<T2>(),
                world.GetPool<T3>()
            );
        }
    }

    public readonly struct EcsSpec<T1, T2, T3, T4> : IFilterGenerator
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsPool<T2> Pool2 { get; }

        public EcsPool<T3> Pool3 { get; }

        public EcsPool<T4> Pool4 { get; }


        public EcsSpec(in EcsSpec<T1, T2, T3> EcsSpec, EcsPool<T4> pool4)
        {
            Pool1 = EcsSpec.Pool1;
            Pool2 = EcsSpec.Pool2;
            Pool3 = EcsSpec.Pool3;
            Pool4 = pool4;
        }

        public EcsSpec(
            EcsPool<T1> pool1,
            EcsPool<T2> pool2,
            EcsPool<T3> pool3,
            EcsPool<T4> pool4
        )
        {
            Pool1 = pool1;
            Pool2 = pool2;
            Pool3 = pool3;
            Pool4 = pool4;
        }

        public EcsFilter.Mask Include(EcsWorld world)
        {
            return world
                .Filter<T1>()
                .Inc<T2>()
                .Inc<T3>()
                .Inc<T4>();
        }

        public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
        {
            return mask
                .Exc<T1>()
                .Exc<T2>()
                .Exc<T3>()
                .Exc<T4>();
        }

        public static EcsSpec<T1, T2, T3, T4> Create(EcsWorld world)
        {
            return new EcsSpec<T1, T2, T3, T4>(
                world.GetPool<T1>(),
                world.GetPool<T2>(),
                world.GetPool<T3>(),
                world.GetPool<T4>()
            );
        }
    }

    public readonly struct EcsSpec<T1, T2, T3, T4, T5> : IFilterGenerator
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsPool<T2> Pool2 { get; }

        public EcsPool<T3> Pool3 { get; }

        public EcsPool<T4> Pool4 { get; }

        public EcsPool<T5> Pool5 { get; }

        public EcsSpec(in EcsSpec<T1, T2, T3, T4> EcsSpec, EcsPool<T5> pool5)
        {
            Pool1 = EcsSpec.Pool1;
            Pool2 = EcsSpec.Pool2;
            Pool3 = EcsSpec.Pool3;
            Pool4 = EcsSpec.Pool4;
            Pool5 = pool5;
        }

        public EcsSpec(
            EcsPool<T1> pool1,
            EcsPool<T2> pool2,
            EcsPool<T3> pool3,
            EcsPool<T4> pool4,
            EcsPool<T5> pool5
        )
        {
            Pool1 = pool1;
            Pool2 = pool2;
            Pool3 = pool3;
            Pool4 = pool4;
            Pool5 = pool5;
        }

        public EcsFilter.Mask Include(EcsWorld world)
        {
            return world
                .Filter<T1>()
                .Inc<T2>()
                .Inc<T3>()
                .Inc<T4>()
                .Inc<T5>();
        }

        public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
        {
            return mask
                .Exc<T1>()
                .Exc<T2>()
                .Exc<T3>()
                .Exc<T4>()
                .Exc<T5>();
        }

        public static EcsSpec<T1, T2, T3, T4, T5> Create(EcsWorld world)
        {
            return new EcsSpec<T1, T2, T3, T4, T5>(
                world.GetPool<T1>(),
                world.GetPool<T2>(),
                world.GetPool<T3>(),
                world.GetPool<T4>(),
                world.GetPool<T5>()
            );
        }
    }

    public readonly struct EcsSpec<T1, T2, T3, T4, T5, T6> : IFilterGenerator
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsPool<T2> Pool2 { get; }

        public EcsPool<T3> Pool3 { get; }

        public EcsPool<T4> Pool4 { get; }

        public EcsPool<T5> Pool5 { get; }

        public EcsPool<T6> Pool6 { get; }

        public EcsSpec(in EcsSpec<T1, T2, T3, T4, T5> EcsSpec, EcsPool<T6> pool6)
        {
            Pool1 = EcsSpec.Pool1;
            Pool2 = EcsSpec.Pool2;
            Pool3 = EcsSpec.Pool3;
            Pool4 = EcsSpec.Pool4;
            Pool5 = EcsSpec.Pool5;
            Pool6 = pool6;
        }

        public EcsSpec(
            EcsPool<T1> pool1,
            EcsPool<T2> pool2,
            EcsPool<T3> pool3,
            EcsPool<T4> pool4,
            EcsPool<T5> pool5,
            EcsPool<T6> pool6)
        {
            Pool1 = pool1;
            Pool2 = pool2;
            Pool3 = pool3;
            Pool4 = pool4;
            Pool5 = pool5;
            Pool6 = pool6;
        }

        public EcsFilter.Mask Include(EcsWorld world)
        {
            return world
                .Filter<T1>()
                .Inc<T2>()
                .Inc<T3>()
                .Inc<T4>()
                .Inc<T5>()
                .Inc<T6>();
        }

        public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
        {
            return mask
                .Exc<T1>()
                .Exc<T2>()
                .Exc<T3>()
                .Exc<T4>()
                .Exc<T5>()
                .Exc<T6>();
        }

        public static EcsSpec<T1, T2, T3, T4, T5, T6> Create(EcsWorld world)
        {
            return new EcsSpec<T1, T2, T3, T4, T5, T6>(
                world.GetPool<T1>(),
                world.GetPool<T2>(),
                world.GetPool<T3>(),
                world.GetPool<T4>(),
                world.GetPool<T5>(),
                world.GetPool<T6>()
            );
        }
    }
}