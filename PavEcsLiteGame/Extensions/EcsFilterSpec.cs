using Leopotam.EcsLite;

namespace PavEcsGame.Extensions
{
    public class EcsFilterSpec<TIncl, TOptional, TExclude>
        where TIncl : struct
        where TOptional : struct
        where TExclude : struct
    {
        private InitData _initData;

        private EcsFilterSpec(InitData initData)
        {
            _initData = initData;
        }

        public EcsWorld World { get; private set; }
        public TIncl Include { get; private set; }
        public TOptional Optional { get; private set; }
        public TExclude Exclude { get; private set; }

        public EcsFilter Filter { get; private set; }

        public void Init(EcsSystems systems)
        {
            var universe = _initData.Universe;
            var include = _initData.Include;
            var optional = _initData.Optional;
            var exclude = _initData.Exclude;

            var world = include.GetWorld(universe, systems);
            var mask = include.Include(world);
            var filter = exclude.Exclude(mask).End();

            World = world;
            Filter = filter;
            Include = include.Create(world);
            Optional = optional.Create(world);
            Exclude = exclude.Create(world);

            _initData = null;
        }

        public static EcsFilterSpec<TIncl, TOptional, TExclude> Create(
            EcsUniverse universe,
            IEcsSpecBuilder<TIncl> include,
            IEcsSpecBuilder<TOptional> optional,
            IEcsSpecBuilder<TExclude> exclude
        )
        {
            var setBuilder = universe.StartSet();
            setBuilder = include.Register(setBuilder);
            setBuilder = optional.Register(setBuilder);
            setBuilder = exclude.Register(setBuilder);
            setBuilder.End();
            var initData = new InitData
            {
                Universe = universe,
                Include = include,
                Optional = optional,
                Exclude = exclude
            };

            return new EcsFilterSpec<TIncl, TOptional, TExclude>(initData);
        }

        private class InitData
        {
            public IEcsSpecBuilder<TExclude> Exclude;
            public IEcsSpecBuilder<TIncl> Include;
            public IEcsSpecBuilder<TOptional> Optional;
            public EcsUniverse Universe;
        }
    }


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

    public static class EcsUniverseExtensions
    {
        public static EcsFilterSpec<TIncl, TOptional, TExclude> CreateFilterSpec<TIncl, TOptional, TExclude>(
            this EcsUniverse universe,
            IEcsSpecBuilder<TIncl> include,
            IEcsSpecBuilder<TOptional> optional,
            IEcsSpecBuilder<TExclude> exclude
        )
            where TIncl : struct
            where TOptional : struct
            where TExclude : struct
        {
            return EcsFilterSpec<TIncl, TOptional, TExclude>.Create(universe, include, optional, exclude);
        }

        public static EcsEntityFactorySpec<TPools> CreateEntityFactorySpec<TPools>(
            this EcsUniverse universe,
            IEcsSpecBuilder<TPools> pools
        )
            where TPools : struct
        {
            return EcsEntityFactorySpec<TPools>.Create(universe, pools);
        }

        public static EcsEntityFactorySpec<TPools> CreateEntityFactorySpec<TPools, TParentPools>(
            this EcsUniverse universe,
            EcsEntityFactorySpec<TParentPools> parent,
            IEcsSpecBuilder<TPools> pools
        )
            where TPools : struct
            where TParentPools : struct
        {
            return EcsEntityFactorySpec<TPools>.Create(universe, pools, parent);
        }
    }
}