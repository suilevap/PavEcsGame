using System;
using System.Collections.Generic;
using System.Diagnostics;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public readonly struct EcsMergeSpec<TReadOnlySpec, TSpec> : IEcsSpec,
        IHasBuilder<EcsMergeSpec<TReadOnlySpec, TSpec>>
        where TReadOnlySpec : struct, IHasBuilder<TReadOnlySpec>, IEcsReadonlySpec
        where TSpec : struct, IHasBuilder<TSpec>, IEcsSpec
    {
        public TReadOnlySpec Readonly { get; }

        public TSpec Write { get; }

        public bool IsBelongToWorld(EcsWorld world)
        {
            return Readonly.IsBelongToWorld(world);
        }

        internal EcsMergeSpec(TReadOnlySpec read, TSpec write)
        {
            Readonly = read;
            Write = write;
        }

        public void Deconstruct(
            out TReadOnlySpec readonlySpec,
            out TSpec writeSpec
        )
        {
            readonlySpec = Readonly;
            writeSpec = Write;
        }

        public static Builder Build()
        {
            return Builder.Instance;
        }

        public IEcsSpecBuilder<EcsMergeSpec<TReadOnlySpec, TSpec>> GetBuilder()
        {
            return Builder.Instance;
        }

        public class Builder : IEcsSpecBuilder<EcsMergeSpec<TReadOnlySpec, TSpec>>
        {
            private readonly IEcsSpecBuilder<TReadOnlySpec> _readBuilder = new TReadOnlySpec().GetBuilder();
            private readonly IEcsSpecBuilder<TSpec> _writeBuilder = new TSpec().GetBuilder();
            internal static Builder Instance { get; } = new Builder();


            public EcsWorld.Mask Include(EcsWorld world)
            {
                var result = _readBuilder.Include(world);
                result = _writeBuilder.Include(result);
                return result;
            }

            public EcsWorld.Mask Include(EcsWorld.Mask mask)
            {
                var result = _readBuilder.Include(mask);
                result = _writeBuilder.Include(result);
                return result;
            }

            public EcsWorld.Mask Exclude(EcsWorld.Mask mask)
            {
                Debug.Assert(true, "Should not be used in exclude section");
                return mask;
            }

            public EcsMergeSpec<TReadOnlySpec, TSpec> Create(EcsWorld world)
            {
                return new EcsMergeSpec<TReadOnlySpec, TSpec>(
                    _readBuilder.Create(world),
                    _writeBuilder.Create(world));
            }

            public IEnumerable<Type> GetArgTypes()
            {
                foreach (var t in _readBuilder.GetArgTypes()) yield return t;
                foreach (var t in _writeBuilder.GetArgTypes()) yield return t;
            }

            public EcsWorld GetWorld(EcsUniverse universe, IEcsSystems systems)
            {
                var world = _readBuilder.GetWorld(universe, systems);
                Debug.Assert(world == _writeBuilder.GetWorld(universe, systems),
                    "Read and write should use the same world");
                return world;
            }
        }
    }
}