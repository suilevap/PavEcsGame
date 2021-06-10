using System;
using System.Linq;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    internal class EcsFilterSpecBuilder<TIncl, TOptional, TExclude> : IInitSpec, IEcsLinkedToWorld
        where TIncl : struct
        where TOptional : struct
        where TExclude : struct
    {
        private InitData _initData;

        private EcsFilterSpecBuilder(InitData initData)
        {
            _initData = initData;
        }

        public EcsWorld World { get; private set; }
        public TIncl Include { get; private set; }
        public TOptional Optional { get; private set; }
        public TExclude Exclude { get; private set; }

        public EcsFilter Filter { get; private set; }

        void IInitSpec.Init(EcsSystems systems)
        {
            var universe = _initData.Universe;
            var include = _initData.Include;
            var optional = _initData.Optional;
            var exclude = _initData.Exclude;

            var world = include.GetWorld(universe, systems);
            var mask = include.Include(world);
            var filter = exclude.Exclude(mask).End();

            World = world;
            Filter = filter;
            Include = include.Create(world);
            Optional = optional.Create(world);
            Exclude = exclude.Create(world);

            _initData = null;
        }

        internal static EcsFilterSpecBuilder<TIncl, TOptional, TExclude> Create(
            EcsUniverseBuilder builder,
            IEcsSystemSpec system,
            IEcsSpecBuilder<TIncl> include,
            IEcsSpecBuilder<TOptional> optional,
            IEcsSpecBuilder<TExclude> exclude
        )
        {

            var args =
                include.GetArgTypes()
                    .Select(x => (x.type, GetPermissionForInclude(x.permission)))
                .Concat(
                    optional.GetArgTypes()
                        .Select(x => (x.type, GetPermissionForOptional(x.permission)))
                    )
                .Concat(
                    exclude.GetArgTypes()
                        .Select(x => (x.type, GetPermissionForExclude(x.permission)))
                );

            SpecPermissions GetPermissionForInclude(SpecPermissions p)
            {
                //include can not create new components
                return p & (SpecPermissions.Read | SpecPermissions.Write);
            }

            SpecPermissions GetPermissionForOptional(SpecPermissions p)
            {
                //optional
                return p.HasFlag(SpecPermissions.Write) 
                    ? p // can do anything
                    : p & SpecPermissions.Read;

            }
            SpecPermissions GetPermissionForExclude(SpecPermissions p)
            {
                //exclude can only create new one
                return p & SpecPermissions.Create;
            }
            builder.RegisterSet(system, args);

            var initData = new InitData
            {
                Universe = builder.Universe,
                Include = include,
                Optional = optional,
                Exclude = exclude
            };

            return new EcsFilterSpecBuilder<TIncl, TOptional, TExclude>(initData);
        }

        private class InitData
        {
            public IEcsSpecBuilder<TExclude> Exclude;
            public IEcsSpecBuilder<TIncl> Include;
            public IEcsSpecBuilder<TOptional> Optional;
            public EcsUniverse Universe;
        }

        public bool IsBelongToWorld(EcsWorld world) => World == world;
    }
}