﻿using System;
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

        void IInitSpec.Init(IEcsSystems systems)
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
            builder.RegisterSet(
                include.GetArgTypes(), 
            Enumerable.Concat(
                    optional.GetArgTypes(),
                    exclude.GetArgTypes()));

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