using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PavEcsSpec.EcsLite
{
    public readonly struct EcsReadonlyPool<T>
        where T : struct
    {
        private readonly EcsPool<T> _pool;

        public EcsReadonlyPool(EcsPool<T> pool)
        {
            _pool = pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T Get(EcsUnsafeEntity entity) => ref _pool.Get(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(EcsUnsafeEntity entity) => _pool.Has(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator EcsReadonlyPool<T>(in EcsPool<T> pool)
        {
            return new EcsReadonlyPool<T>(pool);
        }
    }

    //public readonly struct EcsWritePool<T>
    //    where T : struct
    //{
    //    private readonly EcsPool<T> _pool;

    //    public EcsWritePool(EcsPool<T> pool)
    //    {
    //        _pool = pool;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public ref T Get(EcsUnsafeEntity entity) => ref _pool.Get(entity);

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool Has(EcsUnsafeEntity entity) => _pool.Has(entity);
    //}


    //readonly include -> Get //readonly
    //readonly optional -> Get Has //readonly
    //readonly exclude -> nonsence //---

    //create include -> nonsense //---
    //create optional -> GetOrAdd //write
    //create exclude -> Add //write

    //write include -> Get
    //write optional -> Has, GetOrAdd
    //write exclude -> Add
}
