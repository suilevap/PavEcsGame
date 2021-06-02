using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public interface IEcsSpecBuilder<out T>
        where T : struct
    {
        EcsFilter.Mask Include(EcsWorld world);
        EcsFilter.Mask Include(EcsFilter.Mask mask);
        EcsFilter.Mask Exclude(EcsFilter.Mask mask);

        IEnumerable<Type> GetArgTypes();

        EcsWorld GetWorld(EcsUniverse universe, EcsSystems systems);

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