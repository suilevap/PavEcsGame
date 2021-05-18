using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public readonly struct EcsEntityFactorySpec<TPools> : IEcsLinkedToWorld
        where TPools : struct, IHasBuilder<TPools>
    {
        private readonly EcsEntityFactorySpecBuilder<TPools> _main;

        public EcsWorld World => _main.World;
        public TPools Pools => _main.Pools;
        public EcsUnsafeEntity NewUnsafeEntity()
        {
            return new EcsUnsafeEntity(World.NewEntity());
        }
        public bool IsBelongToWorld(EcsWorld world) => World == world;

        internal EcsEntityFactorySpec(EcsEntityFactorySpecBuilder<TPools> main)
        {
            _main = main;
        }

    }
}