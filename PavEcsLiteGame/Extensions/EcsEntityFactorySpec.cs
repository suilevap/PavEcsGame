﻿using Leopotam.EcsLite;

namespace PavEcsGame.Extensions
{
    public class EcsEntityFactorySpec<TPools>
        where TPools : struct
    {
        private InitData _initData;

        private EcsEntityFactorySpec(InitData initData)
        {
            _initData = initData;
        }

        public EcsWorld World { get; private set; }
        public TPools Pools { get; private set; }

        public void Init(EcsSystems systems)
        {
            var universe = _initData.Universe;
            var pools = _initData.Pools;

            var world = pools.GetWorld(universe, systems);
            World = world;
            Pools = pools.Create(world);

            _initData = null;
        }

        public EcsPackedEntityWithWorld? NewEntity() => 
            World.IsAlive() 
                ? World.PackEntityWithWorld(World.NewEntity()) 
                : default;

        public EcsUnsafeEntity NewUnsafeEntity() => new EcsUnsafeEntity(World.NewEntity());

        public static EcsEntityFactorySpec<TPools> Create(
            EcsUniverse universe,
            IEcsSpecBuilder<TPools> pools
        )
        {
            var setBuilder = universe.StartSet();
            setBuilder = pools.Register(setBuilder);
            setBuilder.End();
            var initData = new InitData
            {
                Universe = universe,
                Pools = pools
            };

            return new EcsEntityFactorySpec<TPools>(initData);
        }

        public static EcsEntityFactorySpec<TPools> Create<TParentPools>(
            EcsUniverse universe,
            IEcsSpecBuilder<TPools> pools,
            EcsEntityFactorySpec<TParentPools> parentFactory
        )
            where TParentPools : struct
        {
            var setBuilder = universe.StartSet();
            setBuilder = pools.Register(setBuilder);
            setBuilder = parentFactory._initData.Pools.Register(setBuilder);
            setBuilder.End();
            var initData = new InitData
            {
                Universe = universe,
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