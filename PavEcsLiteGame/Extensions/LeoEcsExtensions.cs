using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.Ecs;
using Leopotam.EcsLite;
using PavEcsGame.Components;

namespace PavEcsGame.Extensions
{
    public static class LeoEcsExtensions
    {
        public static bool IsEmpty(this EcsFilter filter) => filter.GetEntitiesCount() == 0;

        public static EcsEntityBuilder BuildNewEntity(this EcsWorld world)
        {
            return new EcsEntityBuilder(world, world.NewEntity());
        }

        public static bool IsAlive(this in EcsPackedEntity packed, EcsWorld world)
        {
            return packed.Unpack(world, out _);
        }

    }

    public readonly struct EcsEntityBuilder
    {
        private readonly int _entity;
        private readonly EcsWorld _world;

        public EcsEntityBuilder(EcsWorld world, int entity)
        {
            _world = world;
            _entity = entity;
        }

        public EcsEntityBuilder Add<T>(in T component) where T : struct
        {
            return Add(_world.GetPool<T>(), component);
        }

        public EcsEntityBuilder Add<T>(EcsPool<T> pool, in T component) where T : struct
        {
            pool.Add(_entity) = component;
            return this;
        }

        public EcsEntityBuilder Tag<T>() where T : struct
        {
            return Tag(_world.GetPool<T>());
        }
        public EcsEntityBuilder Tag<T>(EcsPool<T> pool) where T : struct
        {
            pool.Add(_entity);
            return this;
        }

        public EcsPackedEntity End() => _world.PackEntity(_entity);
    }
}
