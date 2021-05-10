using System;
using Leopotam.EcsLite;

namespace PavEcsGame.Extensions
{
    public interface IEcsSpecBuilder<out T>
        where T : struct
    {
        EcsFilter.Mask Include(EcsWorld world);
        EcsFilter.Mask Exclude(EcsFilter.Mask mask);

        EcsUniverse.Builder Register(EcsUniverse.Builder setBuilder);

        EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems);

        T Create(EcsWorld world);
    }

    public readonly struct EcsSpec
    {
        public static Builder Empty()
        {
            return new Builder();
        }

        public struct Builder : IEcsSpecBuilder<EcsSpec>
        {
            public EcsFilter.Mask Include(EcsWorld world)
            {
                throw new InvalidOperationException("Empty spec is impossible to use as include filter");
            }

            public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
            {
                return mask;
            }

            public EcsUniverse.Builder Register(EcsUniverse.Builder setBuilder)
            {
                return setBuilder;
            }

            public EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems)
            {
                return null;
            }

            public EcsSpec Create(EcsWorld world)
            {
                return new EcsSpec();
            }
        }
    }

    public readonly struct EcsSpec<T1>
        where T1 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsSpec(EcsPool<T1> pool1)
        {
            Pool1 = pool1;
        }

        public static Builder Build()
        {
            return new Builder();
        }

        public readonly struct Builder : IEcsSpecBuilder<EcsSpec<T1>>
        {
            public EcsFilter.Mask Include(EcsWorld world)
            {
                return world.Filter<T1>();
            }

            public EcsFilter.Mask Exclude(EcsFilter.Mask mask)
            {
                return mask.Exc<T1>();
            }

            public EcsSpec<T1> Create(EcsWorld world)
            {
                return new EcsSpec<T1>(
                    world.GetPool<T1>()
                );
            }

            public EcsUniverse.Builder Register(EcsUniverse.Builder setBuilder)
            {
                return setBuilder
                    .Add<T1>();
            }

            public EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems)
            {
                return universe.GetWorld<T1>(systems);
            }
        }
    }

    public readonly struct EcsSpec<T1, T2>
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

        public static Builder Build()
        {
            return new Builder();
        }

        public readonly struct Builder : IEcsSpecBuilder<EcsSpec<T1, T2>>
        {
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

            public EcsSpec<T1, T2> Create(EcsWorld world)
            {
                return new EcsSpec<T1, T2>(
                    world.GetPool<T1>(),
                    world.GetPool<T2>()
                );
            }

            public EcsUniverse.Builder Register(EcsUniverse.Builder setBuilder)
            {
                return setBuilder
                    .Add<T1>()
                    .Add<T2>();
            }

            public EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems)
            {
                return universe.GetWorld<T1>(systems);
            }
        }
    }

    public readonly struct EcsSpec<T1, T2, T3>
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

        public static Builder Build()
        {
            return new Builder();
        }

        public readonly struct Builder : IEcsSpecBuilder<EcsSpec<T1, T2, T3>>
        {
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

            public EcsSpec<T1, T2, T3> Create(EcsWorld world)
            {
                return new EcsSpec<T1, T2, T3>(
                    world.GetPool<T1>(),
                    world.GetPool<T2>(),
                    world.GetPool<T3>()
                );
            }


            public EcsUniverse.Builder Register(EcsUniverse.Builder setBuilder)
            {
                return setBuilder
                    .Add<T1>()
                    .Add<T2>()
                    .Add<T3>();
            }

            public EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems)
            {
                return universe.GetWorld<T1>(systems);
            }
        }
    }

    public readonly struct EcsSpec<T1, T2, T3, T4>
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

        public static Builder Build()
        {
            return new Builder();
        }

        public readonly struct Builder : IEcsSpecBuilder<EcsSpec<T1, T2, T3, T4>>
        {
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

            public EcsSpec<T1, T2, T3, T4> Create(EcsWorld world)
            {
                return new EcsSpec<T1, T2, T3, T4>(
                    world.GetPool<T1>(),
                    world.GetPool<T2>(),
                    world.GetPool<T3>(),
                    world.GetPool<T4>()
                );
            }

            public EcsUniverse.Builder Register(EcsUniverse.Builder setBuilder)
            {
                return setBuilder
                    .Add<T1>()
                    .Add<T2>()
                    .Add<T3>()
                    .Add<T4>();
            }

            public EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems)
            {
                return universe.GetWorld<T1>(systems);
            }
        }
    }

    public readonly struct EcsSpec<T1, T2, T3, T4, T5>
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

        public static Builder Build()
        {
            return new Builder();
        }

        public readonly struct Builder : IEcsSpecBuilder<EcsSpec<T1, T2, T3, T4, T5>>
        {
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

            public EcsSpec<T1, T2, T3, T4, T5> Create(EcsWorld world)
            {
                return new EcsSpec<T1, T2, T3, T4, T5>(
                    world.GetPool<T1>(),
                    world.GetPool<T2>(),
                    world.GetPool<T3>(),
                    world.GetPool<T4>(),
                    world.GetPool<T5>()
                );
            }


            public EcsUniverse.Builder Register(EcsUniverse.Builder setBuilder)
            {
                return setBuilder
                    .Add<T1>()
                    .Add<T2>()
                    .Add<T3>()
                    .Add<T4>()
                    .Add<T5>();
            }

            public EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems)
            {
                return universe.GetWorld<T1>(systems);
            }
        }
    }

    public readonly struct EcsSpec<T1, T2, T3, T4, T5, T6>
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

        public static Builder Build()
        {
            return new Builder();
        }

        public readonly struct Builder : IEcsSpecBuilder<EcsSpec<T1, T2, T3, T4, T5, T6>>
        {
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

            public EcsSpec<T1, T2, T3, T4, T5, T6> Create(EcsWorld world)
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

            public EcsUniverse.Builder Register(EcsUniverse.Builder setBuilder)
            {
                return setBuilder
                    .Add<T1>()
                    .Add<T2>()
                    .Add<T3>()
                    .Add<T4>()
                    .Add<T5>()
                    .Add<T6>();
            }

            public EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems)
            {
                return universe.GetWorld<T1>(systems);
            }
        }
    }
}