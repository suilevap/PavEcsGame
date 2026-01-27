using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public interface IEcsSpecBuilder<out T>
        where T : struct
    {
        EcsWorld.Mask Include(EcsWorld world);
        EcsWorld.Mask Include(EcsWorld.Mask mask);
        EcsWorld.Mask Exclude(EcsWorld.Mask mask);

        IEnumerable<Type> GetArgTypes();

        EcsWorld GetWorld(EcsUniverse universe, IEcsSystems systems);

        T Create(EcsWorld world);
    }

    public interface IEcsLinkedToWorld
    {
        bool IsBelongToWorld(EcsWorld world);
    }

    public interface IHasBuilder<out T>
        where T : struct
    {
        IEcsSpecBuilder<T> GetBuilder();
    }
}