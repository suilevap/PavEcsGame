using System;
using Leopotam.EcsLite;

namespace PavEcsGame.Extensions
{
    public readonly struct EcsFilterSpec<TIncl, TOptional, TExclude>
        where TIncl : struct
        where TOptional : struct
        where TExclude : struct
    {
        public readonly TIncl Include;
        public readonly TOptional Optional;
        public readonly TExclude Exclude;

        public readonly EcsFilter Filter;

        public EcsFilterSpec(EcsFilter filter, TIncl include, TOptional optional, TExclude exclude)
        {
            Include = include;
            Optional = optional;
            Exclude = exclude;
            Filter = filter;
        }
    }

    public static class EcsFilterSpecBuilder
    {
        public static EcsFilterSpec<TIncl, TOptional, TExclude> CreateFilterSpec<TIncl, TOptional, TExclude>(
            this EcsWorld world,
            Func<EcsWorld, TIncl> include,
            Func<EcsWorld, TOptional> optional,
            Func<EcsWorld, TExclude> exclude
        )
            where TIncl : struct, IFilterGenerator
            where TOptional : struct, IFilterGenerator
            where TExclude : struct, IFilterGenerator
        {
            var includeData = include(world);
            var optionalData = optional(world);
            var excludeData = exclude(world);
            return world.CreateFilterSpec(includeData, optionalData, excludeData);
        }

        public static EcsFilterSpec<TIncl, TOptional, TExclude> CreateFilterSpec<TIncl, TOptional, TExclude>(
            this EcsWorld world,
            TIncl include,
            TOptional optional,
            TExclude exclude
        )
            where TIncl : struct, IFilterGenerator
            where TOptional : struct, IFilterGenerator
            where TExclude : struct, IFilterGenerator
        {
            var mask = include.Include(world);
            var filter = exclude.Exclude(mask).End();

            return new EcsFilterSpec<TIncl, TOptional, TExclude>(filter, include, optional, exclude);
        }
    }
}