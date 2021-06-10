using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    [Flags]
    public enum SpecPermissions
    {
        None = 0,
        Read = 1,
        Write = 2,
        Create = 4, 
        CreateOrAlter = Create | Write,
        Full = Read | Write | CreateOrAlter
    }
    public interface IEcsSpecBuilder<out T>
        where T : struct
    {
        EcsFilter.Mask Include(EcsWorld world);
        EcsFilter.Mask Include(EcsFilter.Mask mask);
        EcsFilter.Mask Exclude(EcsFilter.Mask mask);

        IEnumerable<(Type type, SpecPermissions permission)> GetArgTypes();

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