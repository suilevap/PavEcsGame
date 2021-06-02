using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    //TODO: autogenerate
    public readonly struct EcsFilterSpec<TIncl, TOptional, TExclude> : IEcsLinkedToWorld
        where TIncl : struct, IHasBuilder<TIncl>, IEcsSpec
        where TOptional : struct, IHasBuilder<TOptional>, IEcsSpec
        where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
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
            where TIncl : struct, IHasBuilder<TIncl>, IEcsSpec
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
                where TOptional : struct, IHasBuilder<TOptional>, IEcsSpec
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
                    where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
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
                where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
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



        public readonly struct Inc<TInclReadonly, TIncl> : IEcsLinkedToWorld
            where TIncl : struct, IHasBuilder<TIncl>, IEcsSpec
            where TInclReadonly : struct, IHasBuilder<TInclReadonly>, IEcsReadonlySpec
        {
            private readonly EcsFilterSpecBuilder<EcsMergeSpec<TInclReadonly, TIncl>, EcsSpec, EcsSpec> _main;

            public EcsWorld World => _main.World;
            public TInclReadonly IncludeReadonly => _main.Include.Readonly;

            public TIncl Include => _main.Include.Write;

            public EcsFilter Filter => _main.Filter;

            public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

            internal Inc(EcsFilterSpecBuilder<EcsMergeSpec<TInclReadonly, TIncl>, EcsSpec, EcsSpec> main)
            {
                _main = main;
            }

            public readonly struct Opt<TOptional> : IEcsLinkedToWorld
                where TOptional : struct, IHasBuilder<TOptional>, IEcsSpec
            {
                private readonly EcsFilterSpecBuilder<EcsMergeSpec<TInclReadonly, TIncl>, TOptional, EcsSpec> _main;

                public EcsWorld World => _main.World;
                public TInclReadonly IncludeReadonly => _main.Include.Readonly;
                public TIncl Include => _main.Include.Write;
                public TOptional Optional => _main.Optional;

                public EcsFilter Filter => _main.Filter;

                public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

                internal Opt(EcsFilterSpecBuilder<EcsMergeSpec<TInclReadonly, TIncl>, TOptional, EcsSpec> main)
                {
                    _main = main;
                }

                public readonly struct Exc<TExclude> : IEcsLinkedToWorld
                    where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
                {
                    private readonly EcsFilterSpecBuilder<EcsMergeSpec<TInclReadonly, TIncl>, TOptional, TExclude> _main;

                    public TInclReadonly IncludeReadonly => _main.Include.Readonly;
                    public TIncl Include => _main.Include.Write;
                    public TOptional Optional => _main.Optional;
                    public TExclude Exclude => _main.Exclude;

                    public EcsFilter Filter => _main.Filter;

                    public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

                    internal Exc(EcsFilterSpecBuilder<EcsMergeSpec<TInclReadonly, TIncl>, TOptional, TExclude> main)
                    {
                        _main = main;
                    }
                }
            }

            public readonly struct Exc<TExclude> : IEcsLinkedToWorld
                where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
            {
                private readonly EcsFilterSpecBuilder<EcsMergeSpec<TInclReadonly, TIncl>, EcsSpec, TExclude> _main;

                public EcsWorld World => _main.World;
                public TInclReadonly IncludeReadonly => _main.Include.Readonly;
                public TIncl Include => _main.Include.Write;
                public TExclude Exclude => _main.Exclude;

                public EcsFilter Filter => _main.Filter;

                public bool IsBelongToWorld(EcsWorld world) => _main.IsBelongToWorld(world);

                internal Exc(EcsFilterSpecBuilder<EcsMergeSpec<TInclReadonly, TIncl>, EcsSpec, TExclude> main)
                {
                    _main = main;
                }

            }
        }

    }

}