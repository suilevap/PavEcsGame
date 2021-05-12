using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public class EcsFilterSpec<TIncl, TOptional, TExclude> : IInitSpec
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

        void IInitSpec.Init(EcsSystems systems)
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

        internal static EcsFilterSpec<TIncl, TOptional, TExclude> Create(
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
}