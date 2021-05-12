using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public class EcsEntityFactorySpec<TPools> : IInitSpec
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

        internal static EcsEntityFactorySpec<TPools> Create(
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

        internal static EcsEntityFactorySpec<TPools> Create<TParentPools>(
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