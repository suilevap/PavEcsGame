using System;
using System.Linq;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public class EcsEntityFactorySpec<TPools> : IInitSpec, IEcsLinkedToWorld
        where TPools : struct
    {
        private InitData _initData;

        private EcsEntityFactorySpec(InitData initData)
        {
            _initData = initData;
        }

        public EcsWorld World { get; private set; }
        public TPools Pools { get; private set; }

        void IInitSpec.Init(EcsSystems systems)
        {
            var universe = _initData.Universe;
            var pools = _initData.Pools;

            var world = pools.GetWorld(universe, systems);
            World = world;
            Pools = pools.Create(world);

            _initData = null;
        }

        public EcsPackedEntityWithWorld? NewEntity()
        {
            return World.IsAlive()
                ? World.PackEntityWithWorld(World.NewEntity())
                : default;
        }

        public EcsUnsafeEntity NewUnsafeEntity()
        {
            return new EcsUnsafeEntity(World.NewEntity());
        }
        public bool IsBelongToWorld(EcsWorld world) => World == world;

        internal static EcsEntityFactorySpec<TPools> Create(
            EcsUniverseBuilder builder,
            IEcsSpecBuilder<TPools> pools
        )
        {
            builder.RegisterSet(pools.GetArgTypes(), Enumerable.Empty<Type>());
            var initData = new InitData
            {
                Universe = builder.Universe,
                Pools = pools
            };

            return new EcsEntityFactorySpec<TPools>(initData);
        }

        internal static EcsEntityFactorySpec<TPools> Create<TParentPools>(
            EcsUniverseBuilder builder,
            IEcsSpecBuilder<TPools> pools,
            EcsEntityFactorySpec<TParentPools> parentFactory
        )
            where TParentPools : struct
        {
            var required = 
                Enumerable.Concat(
                    pools.GetArgTypes(),
                    parentFactory._initData.Pools.GetArgTypes());
            builder.RegisterSet(required, Enumerable.Empty<Type>());

            var initData = new InitData
            {
                Universe = builder.Universe,
                Pools = pools
            };

            return new EcsEntityFactorySpec<TPools>(initData);
        }

        private class InitData
        {
            public IEcsSpecBuilder<TPools> Pools;
            public EcsUniverse Universe;
        }
    }
}