namespace PavEcsSpec.EcsLite
{
    public static class EcsUniverseExtensions
    {

        public static EcsUniverse Build<TInclude, TOptional, TExclude>(
            this EcsUniverse universe,
            ref EcsFilterSpec<TInclude, TOptional, TExclude> spec)

            where TInclude : struct, IHasBuilder<TInclude>
            where TOptional : struct, IHasBuilder<TOptional>
            where TExclude : struct, IHasBuilder<TExclude>
        {
            var includeBuilder = (new TInclude()).GetBuilder();
            var optionalBuilder = (new TOptional()).GetBuilder();
            var excludeBuilder = (new TExclude()).GetBuilder();
            var main = universe.CreateFilterSpec(includeBuilder, optionalBuilder, excludeBuilder);
            spec = new EcsFilterSpec<TInclude, TOptional, TExclude>(main);
            return universe;
        }

        public static EcsUniverse Build<TPools>(
            this EcsUniverse universe,
            ref EcsEntityFactorySpec<TPools> spec)
            where TPools : struct, IHasBuilder<TPools>
        {
            var poolsBuilder = (new TPools()).GetBuilder();
            var main = universe.CreateEntityFactorySpec( poolsBuilder);
            spec = new EcsEntityFactorySpec<TPools>(main);
            return universe;
        }

        public static EcsUniverse Build<TPools,TPools2>(
            this EcsUniverse universe,
            in EcsEntityFactorySpec<TPools2> parentSpec,
            ref EcsEntityFactorySpec<TPools> spec)
            where TPools : struct, IHasBuilder<TPools>
            where TPools2 : struct, IHasBuilder<TPools2>
        {
            var poolsBuilder = (new TPools()).GetBuilder();
            var parentPoolsBuilder = (new TPools2()).GetBuilder();
            var main = universe.CreateEntityFactorySpec(poolsBuilder, parentPoolsBuilder);
            spec = new EcsEntityFactorySpec<TPools>(main);

            return universe;
        }

    }
}
