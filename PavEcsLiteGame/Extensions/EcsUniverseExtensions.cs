namespace PavEcsGame.Extensions
{
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