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
    }
}
