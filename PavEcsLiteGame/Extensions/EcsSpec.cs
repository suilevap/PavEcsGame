﻿using System;
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

    public interface IEcsLinkedToWorld
    {
        bool IsBelongToWorld(EcsWorld world);
    }

    public readonly struct EcsSpec : IEcsLinkedToWorld
    {
        public bool IsBelongToWorld(EcsWorld world) => true;

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

    public readonly struct EcsSpec<T1> : IEcsLinkedToWorld
        where T1 : struct
    {
        private readonly EcsWorld _world;
        public bool IsBelongToWorld(EcsWorld world) => _world == world;

        public EcsPool<T1> Pool1 { get; }

        public EcsSpec(EcsWorld world)
        {
            _world = world;
            Pool1 = world.GetPool<T1>();
        }
        public void Deconstruct(out EcsPool<T1> pool1)
        {
            pool1 = Pool1;
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
                return new EcsSpec<T1>(world);
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

    public readonly struct EcsSpec<T1, T2> : IEcsLinkedToWorld
        where T1 : struct
        where T2 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsPool<T2> Pool2 { get; }

        private readonly EcsWorld _world;
        public bool IsBelongToWorld(EcsWorld world) => _world == world;
        //public EcsSpec(in EcsSpec<T1> ecsSpec, EcsPool<T2> pool2)
        //{
        //    Pool1 = ecsSpec.Pool1;
        //    Pool2 = pool2;
        //    _world = ecsSpec._world;
        //}

        public EcsSpec(
            EcsWorld world)
        {
            _world = world;
            Pool1 = world.GetPool<T1>();
            Pool2 = world.GetPool<T2>();
        }

        public void Deconstruct(
            out EcsPool<T1> pool1,
            out EcsPool<T2> pool2
            )
        {
            pool1 = Pool1;
            pool2 = Pool2;
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
                return new EcsSpec<T1, T2>(world);
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

    public readonly struct EcsSpec<T1, T2, T3> : IEcsLinkedToWorld
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsPool<T2> Pool2 { get; }

        public EcsPool<T3> Pool3 { get; }

        private readonly EcsWorld _world;
        public bool IsBelongToWorld(EcsWorld world) => _world == world;
        public EcsSpec(EcsWorld world)
        {
            _world = world;
            Pool1 = world.GetPool<T1>();
            Pool2 = world.GetPool<T2>();
            Pool3 = world.GetPool<T3>();
        }

        public void Deconstruct(
            out EcsPool<T1> pool1,
            out EcsPool<T2> pool2,
            out EcsPool<T3> pool3
        )
        {
            pool1 = Pool1;
            pool2 = Pool2;
            pool3 = Pool3;
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
                return new EcsSpec<T1, T2, T3>(world);
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

    public readonly struct EcsSpec<T1, T2, T3, T4> : IEcsLinkedToWorld
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        public EcsPool<T1> Pool1 { get; }

        public EcsPool<T2> Pool2 { get; }

        public EcsPool<T3> Pool3 { get; }

        public EcsPool<T4> Pool4 { get; }

        private readonly EcsWorld _world;
        public bool IsBelongToWorld(EcsWorld world) => _world == world;
        public EcsSpec(EcsWorld world)
        {
            _world = world;
            Pool1 = world.GetPool<T1>();
            Pool2 = world.GetPool<T2>();
            Pool3 = world.GetPool<T3>();
            Pool4 = world.GetPool<T4>();
        }

        public void Deconstruct(
            out EcsPool<T1> pool1,
            out EcsPool<T2> pool2,
            out EcsPool<T3> pool3,
            out EcsPool<T4> pool4
        )
        {
            pool1 = Pool1;
            pool2 = Pool2;
            pool3 = Pool3;
            pool4 = Pool4;
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
                return new EcsSpec<T1, T2, T3, T4>(world);
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

    public readonly struct EcsSpec<T1, T2, T3, T4, T5> : IEcsLinkedToWorld
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

        private readonly EcsWorld _world;
        public bool IsBelongToWorld(EcsWorld world) => _world == world;
        public EcsSpec(EcsWorld world)
        {
            _world = world;
            Pool1 = world.GetPool<T1>();
            Pool2 = world.GetPool<T2>();
            Pool3 = world.GetPool<T3>();
            Pool4 = world.GetPool<T4>();
            Pool5 = world.GetPool<T5>();
        }

        public void Deconstruct(
            out EcsPool<T1> pool1,
            out EcsPool<T2> pool2,
            out EcsPool<T3> pool3,
            out EcsPool<T4> pool4,
            out EcsPool<T5> pool5
        )
        {
            pool1 = Pool1;
            pool2 = Pool2;
            pool3 = Pool3;
            pool4 = Pool4;
            pool5 = Pool5;
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
                return new EcsSpec<T1, T2, T3, T4, T5>(world);
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

    public readonly struct EcsSpec<T1, T2, T3, T4, T5, T6> : IEcsLinkedToWorld
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

        private readonly EcsWorld _world;
        public bool IsBelongToWorld(EcsWorld world) => _world == world;
        public EcsSpec(EcsWorld world)
        {
            _world = world;
            Pool1 = world.GetPool<T1>();
            Pool2 = world.GetPool<T2>();
            Pool3 = world.GetPool<T3>();
            Pool4 = world.GetPool<T4>();
            Pool5 = world.GetPool<T5>();
            Pool6 = world.GetPool<T6>();
        }

        public void Deconstruct(
            out EcsPool<T1> pool1,
            out EcsPool<T2> pool2,
            out EcsPool<T3> pool3,
            out EcsPool<T4> pool4,
            out EcsPool<T5> pool5,
            out EcsPool<T6> pool6
        )
        {
            pool1 = Pool1;
            pool2 = Pool2;
            pool3 = Pool3;
            pool4 = Pool4;
            pool5 = Pool5;
            pool6 = Pool6;
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
                return new EcsSpec<T1, T2, T3, T4, T5, T6>(world);
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