using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public readonly struct EcsFilterSpec<TIncl, TOptional, TExclude> : IEcsLinkedToWorld
        where TIncl : struct, IHasBuilder<TIncl>
        where TOptional : struct, IHasBuilder<TOptional>
        where TExclude : struct, IHasBuilder<TExclude>
    {
        private readonly EcsFilterSpecBuilder<TIncl, TOptional, TExclude> _main;

        public EcsWorld World => _main.World;
        public TIncl Include => _main.Include;
        public TOptional Optional => _main.Optional;
        public TExclude Exclude => _main.Exclude;

        public EcsFilter Filter => _main.Filter;

        public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

        internal EcsFilterSpec(EcsFilterSpecBuilder<TIncl, TOptional, TExclude> main)
        {
            _main = main;
        }
    }
    public readonly struct EcsFilterSpec
    {
        public readonly struct Inc<TIncl> : IEcsLinkedToWorld
            where TIncl : struct, IHasBuilder<TIncl>
        {
            private readonly EcsFilterSpecBuilder<TIncl, EcsSpec, EcsSpec> _main;

            public EcsWorld World => _main.World;
            public TIncl Include => _main.Include;

            public EcsFilter Filter => _main.Filter;

            public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

            internal Inc(EcsFilterSpecBuilder<TIncl, EcsSpec, EcsSpec> main)
            {
                _main = main;
            }

            public readonly struct Opt<TOptional> : IEcsLinkedToWorld
                where TOptional : struct, IHasBuilder<TOptional>
            {
                private readonly EcsFilterSpecBuilder<TIncl, TOptional, EcsSpec> _main;

                public EcsWorld World => _main.World;
                public TIncl Include => _main.Include;
                public TOptional Optional => _main.Optional;

                public EcsFilter Filter => _main.Filter;

                public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

                internal Opt(EcsFilterSpecBuilder<TIncl, TOptional, EcsSpec> main)
                {
                    _main = main;
                }

                public readonly struct Exc<TExclude> : IEcsLinkedToWorld
                    where TExclude : struct, IHasBuilder<TExclude>
                {
                    private readonly EcsFilterSpecBuilder<TIncl, TOptional, TExclude> _main;

                    public EcsWorld World => _main.World;
                    public TIncl Include => _main.Include;
                    public TOptional Optional => _main.Optional;
                    public TExclude Exclude => _main.Exclude;

                    public EcsFilter Filter => _main.Filter;

                    public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

                    internal Exc(EcsFilterSpecBuilder<TIncl, TOptional, TExclude> main)
                    {
                        _main = main;
                    }
                }
            }

            public readonly struct Exc<TExclude> : IEcsLinkedToWorld
                where TExclude : struct, IHasBuilder<TExclude>
            {
                private readonly EcsFilterSpecBuilder<TIncl, EcsSpec, TExclude> _main;

                public EcsWorld World => _main.World;
                public TIncl Include => _main.Include;
                public TExclude Exclude => _main.Exclude;

                public EcsFilter Filter => _main.Filter;

                public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

                internal Exc(EcsFilterSpecBuilder<TIncl, EcsSpec, TExclude> main)
                {
                    _main = main;
                }

            }
        }
    }
}