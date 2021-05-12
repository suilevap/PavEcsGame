namespace PavEcsSpec.EcsLite
{
    public static class EcsUniverseExtensions
    {
        public static EcsFilterSpec<TIncl, TOptional, EcsSpec> CreateFilterSpec<TIncl, TOptional>(
            this EcsUniverse universe,
            IEcsSpecBuilder<TIncl> include,
            IEcsSpecBuilder<TOptional> optional
        )
            where TIncl : struct
            where TOptional : struct
        {
            return universe.CreateFilterSpec(
                include,
                optional,
                EcsSpec.Empty());
        }

        public static EcsFilterSpec<TIncl, EcsSpec, EcsSpec> CreateFilterSpec<TIncl>(
            this EcsUniverse universe,
            IEcsSpecBuilder<TIncl> include
        )
            where TIncl : struct
        {
            return universe.CreateFilterSpec(
                include,
                EcsSpec.Empty(),
                EcsSpec.Empty());
        }

        public static FilterBuilder<TIncl, EcsSpec, EcsSpec> StartFilterSpec<TIncl>(
            this EcsUniverse universe,
            IEcsSpecBuilder<TIncl> include
        )
            where TIncl : struct

        {
            return new FilterBuilder<TIncl, EcsSpec, EcsSpec>(
                universe,
                include,
                EcsSpec.Empty(),
                EcsSpec.Empty());
        }


        public static FilterBuilder<TIncl, TOpt, TExcl> Optional<TIncl, TOpt, TExcl>(
            this FilterBuilder<TIncl, EcsSpec, TExcl> builder,
            IEcsSpecBuilder<TOpt> optional
        )
            where TIncl : struct
            where TOpt : struct
            where TExcl : struct
        {
            return new FilterBuilder<TIncl, TOpt, TExcl>(
                builder.Universe,
                builder.Include,
                optional,
                builder.Exclude);
        }

        public static FilterBuilder<TIncl, TOpt, TExcl> Exclude<TIncl, TOpt, TExcl>(
            this FilterBuilder<TIncl, TOpt, EcsSpec> builder,
            IEcsSpecBuilder<TExcl> exclude
        )
            where TIncl : struct
            where TOpt : struct
            where TExcl : struct
        {
            return new FilterBuilder<TIncl, TOpt, TExcl>(
                builder.Universe,
                builder.Include,
                builder.Optional,
                exclude);

        }

        public static EcsFilterSpec<TIncl, TOpt, TExcl> End<TIncl, TOpt, TExcl>(
            this FilterBuilder<TIncl, TOpt, TExcl> builder
        )
            where TIncl : struct
            where TOpt : struct
            where TExcl : struct
        {
            return builder.Universe.CreateFilterSpec(builder.Include, builder.Optional, builder.Exclude);
        }

        public readonly struct FilterBuilder<TIncl, TOpt, TExcl>
             where TIncl : struct
             where TOpt : struct
             where TExcl : struct
        {
            internal readonly IEcsSpecBuilder<TIncl> Include;
            internal readonly IEcsSpecBuilder<TOpt> Optional;
            internal readonly IEcsSpecBuilder<TExcl> Exclude;
            internal readonly EcsUniverse Universe;


            public FilterBuilder(
                EcsUniverse universe,
                IEcsSpecBuilder<TIncl> include,
                IEcsSpecBuilder<TOpt> optional,
                IEcsSpecBuilder<TExcl> exclude)
            {
                Include = include;
                Optional = optional;
                Exclude = exclude;
                Universe = universe;
            }
        }
    }
}
