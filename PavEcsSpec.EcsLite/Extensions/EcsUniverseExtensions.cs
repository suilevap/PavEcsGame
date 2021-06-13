using System.Diagnostics;
using System.Text;
using System.Linq;


namespace PavEcsSpec.EcsLite
{
    public static class EcsUniverseExtensions
    {
        public static EcsSystemSpecRegister Register(this EcsUniverse universe, IEcsSystemSpec system)
        {
            return new EcsSystemSpecRegister(universe, system);
        }

        public class EcsSystemSpecRegister
        {
            private IEcsSystemSpec _system;
            private EcsUniverse _universe;

            public EcsSystemSpecRegister(EcsUniverse universe, IEcsSystemSpec system)
            {
                _universe = universe;
                _system = system;
            }

            public EcsSystemSpecRegister Build<TInclude, TOptional, TExclude>(
                ref EcsFilterSpec<TInclude, TOptional, TExclude> spec)

                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
                where TOptional : struct, IHasBuilder<TOptional>, IEcsSpec
                where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
            {
                var main = GetFilterBuilder<TInclude, TOptional, TExclude>();
                spec = new EcsFilterSpec<TInclude, TOptional, TExclude>(main);
                return this;
            }

            private EcsFilterSpecBuilder<TInclude, TOptional, TExclude> GetFilterBuilder
                <TInclude, TOptional, TExclude>()
                where TInclude : struct, IHasBuilder<TInclude>
                where TOptional : struct, IHasBuilder<TOptional>
                where TExclude : struct, IHasBuilder<TExclude>
            {
                var includeBuilder = (new TInclude()).GetBuilder();
                var optionalBuilder = (new TOptional()).GetBuilder();
                var excludeBuilder = (new TExclude()).GetBuilder();
                var main = _universe.CreateFilterSpec(_system, includeBuilder, optionalBuilder, excludeBuilder);
                return main;
            }

            public EcsSystemSpecRegister Build<TInclude>(
                ref EcsFilterSpec.Inc<TInclude> spec)
                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
            {
                var main = GetFilterBuilder<TInclude, EcsSpec, EcsSpec>();
                spec = new EcsFilterSpec.Inc<TInclude>(main);
                return this;
            }

            public EcsSystemSpecRegister Build<TInclude, TOptional>(
                ref EcsFilterSpec.Inc<TInclude>.Opt<TOptional> spec)
                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
                where TOptional : struct, IHasBuilder<TOptional>, IEcsSpec
            {
                var main = GetFilterBuilder<TInclude, TOptional, EcsSpec>();
                spec = new EcsFilterSpec.Inc<TInclude>.Opt<TOptional>(main);
                return this;
            }

            public EcsSystemSpecRegister Build<TInclude, TOptional, TExclude>(

                ref EcsFilterSpec.Inc<TInclude>.Opt<TOptional>.Exc<TExclude> spec)
                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
                where TOptional : struct, IHasBuilder<TOptional>, IEcsSpec
                where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
            {
                var main = GetFilterBuilder<TInclude, TOptional, TExclude>();
                spec = new EcsFilterSpec.Inc<TInclude>.Opt<TOptional>.Exc<TExclude>(main);
                return this;
            }

            public EcsSystemSpecRegister Build<TInclude, TExclude>(

                ref EcsFilterSpec.Inc<TInclude>.Exc<TExclude> spec)
                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
                where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
            {
                var main = GetFilterBuilder<TInclude, EcsSpec, TExclude>();
                spec = new EcsFilterSpec.Inc<TInclude>.Exc<TExclude>(main);
                return this;
            }

            public EcsSystemSpecRegister Build<TReadonlyInclude, TInclude>(

                ref EcsFilterSpec.Inc<TReadonlyInclude, TInclude> spec)
                where TReadonlyInclude : struct, IHasBuilder<TReadonlyInclude>, IEcsReadonlySpec
                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
            {
                var main = GetFilterBuilder<EcsMergeSpec<TReadonlyInclude, TInclude>, EcsSpec, EcsSpec>();
                spec = new EcsFilterSpec.Inc<TReadonlyInclude, TInclude>(main);
                return this;
            }

            public EcsSystemSpecRegister Build<TReadonlyInclude, TInclude, TOptional>(

                ref EcsFilterSpec.Inc<TReadonlyInclude, TInclude>.Opt<TOptional> spec)
                where TReadonlyInclude : struct, IHasBuilder<TReadonlyInclude>, IEcsReadonlySpec
                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
                where TOptional : struct, IHasBuilder<TOptional>, IEcsSpec
            {
                var main = GetFilterBuilder<EcsMergeSpec<TReadonlyInclude, TInclude>, TOptional, EcsSpec>();
                spec = new EcsFilterSpec.Inc<TReadonlyInclude, TInclude>.Opt<TOptional>(main);
                return this;
            }

            public EcsSystemSpecRegister Build<TReadonlyInclude, TInclude, TOptional, TExclude>(

                ref EcsFilterSpec.Inc<TReadonlyInclude, TInclude>.Opt<TOptional>.Exc<TExclude> spec)
                where TReadonlyInclude : struct, IHasBuilder<TReadonlyInclude>, IEcsReadonlySpec
                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
                where TOptional : struct, IHasBuilder<TOptional>, IEcsSpec
                where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
            {
                var main = GetFilterBuilder<EcsMergeSpec<TReadonlyInclude, TInclude>, TOptional, TExclude>();
                spec = new EcsFilterSpec.Inc<TReadonlyInclude, TInclude>.Opt<TOptional>.Exc<TExclude>(main);
                return this;
            }

            public EcsSystemSpecRegister Build<TReadonlyInclude, TInclude, TExclude>(

                ref EcsFilterSpec.Inc<TReadonlyInclude, TInclude>.Exc<TExclude> spec)
                where TReadonlyInclude : struct, IHasBuilder<TReadonlyInclude>, IEcsReadonlySpec
                where TInclude : struct, IHasBuilder<TInclude>, IEcsSpec
                where TExclude : struct, IHasBuilder<TExclude>, IEcsSpec
            {
                var main = GetFilterBuilder<EcsMergeSpec<TReadonlyInclude, TInclude>, EcsSpec, TExclude>();
                spec = new EcsFilterSpec.Inc<TReadonlyInclude, TInclude>.Exc<TExclude>(main);
                return this;
            }


            public EcsSystemSpecRegister Build<TPools>(

                ref EcsEntityFactorySpec<TPools> spec)
                where TPools : struct, IHasBuilder<TPools>
            {
                var poolsBuilder = (new TPools()).GetBuilder();
                var main = _universe.CreateEntityFactorySpec(_system, poolsBuilder);
                spec = new EcsEntityFactorySpec<TPools>(main);
                return this;
            }

            public EcsSystemSpecRegister Build<TPools, TPools2>(

                in EcsEntityFactorySpec<TPools2> parentSpec,
                ref EcsEntityFactorySpec<TPools> spec)
                where TPools : struct, IHasBuilder<TPools>
                where TPools2 : struct, IHasBuilder<TPools2>
            {
                var poolsBuilder = (new TPools()).GetBuilder();
                var parentPoolsBuilder = (new TPools2()).GetBuilder();
                var main = _universe.CreateEntityFactorySpec(_system, poolsBuilder, parentPoolsBuilder);
                spec = new EcsEntityFactorySpec<TPools>(main);

                return this;
            }
        }

    }
}
