﻿using System;
using System.Linq;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    internal class EcsEntityFactorySpecBuilder<TPools> : IInitSpec, IEcsLinkedToWorld
        where TPools : struct
    {
        private InitData _initData;

        private EcsEntityFactorySpecBuilder(InitData initData)
        {
            _initData = initData;
        }

        public EcsWorld World { get; private set; }
        public TPools Pools { get; private set; }

        void IInitSpec.Init(EcsSystems systems)
        {
            var universe = _initData.Universe;
            var pools = _initData.Pools;

            var world = pools.GetWorld(universe, systems);
            World = world;
            Pools = pools.Create(world);

            _initData = null;
        }

        public EcsPackedEntityWithWorld? NewEntity()
        {
            return World.IsAlive()
                ? World.PackEntityWithWorld(World.NewEntity())
                : default;
        }

        public EcsUnsafeEntity NewUnsafeEntity()
        {
            return new EcsUnsafeEntity(World.NewEntity());
        }
        public bool IsBelongToWorld(EcsWorld world) => World == world;

        internal static EcsEntityFactorySpecBuilder<TPools> Create(
            EcsUniverseBuilder builder,
            IEcsSystemSpec system,
            IEcsSpecBuilder<TPools> pools
        )
        {
            var args = pools.GetArgTypes()
                .Select(x => 
                    (x.type, 
                    x.permission.HasFlag(SpecPermissions.Write) 
                        ? SpecPermissions.CreateOrAlter
                        : SpecPermissions.Read));
            builder.RegisterSet(system, args);

            var initData = new InitData
            {
                Universe = builder.Universe,
                Pools = pools
            };

            return new EcsEntityFactorySpecBuilder<TPools>(initData);
        }

        internal static EcsEntityFactorySpecBuilder<TPools> Create<TParentPools>(
            EcsUniverseBuilder builder,
            IEcsSystemSpec system,
            IEcsSpecBuilder<TPools> pools,
            IEcsSpecBuilder<TParentPools> parentPools
        )
            where TParentPools : struct
        {
            var args =
                Enumerable.Concat(
                    pools.GetArgTypes(),
                    parentPools.GetArgTypes()
                )
                .Select(x => (x.type, SpecPermissions.CreateOrAlter));
            builder.RegisterSet(system, args);

            var initData = new InitData
            {
                Universe = builder.Universe,
                Pools = pools
            };

            return new EcsEntityFactorySpecBuilder<TPools>(initData);
        }

        private class InitData
        {
            public IEcsSpecBuilder<TPools> Pools;
            public EcsUniverse Universe;
        }
    }
}