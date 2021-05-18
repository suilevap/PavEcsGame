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
}