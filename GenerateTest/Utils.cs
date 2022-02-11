using Leopotam.EcsLite;

namespace GenerateTest
{

    public static class LeoExtension
    {
        public static ref T Ensure<T>(this EcsPool<T> pool, EcsUnsafeEntity ent, out bool isNew)
            where T : struct
        {
            if (pool.Has(ent))
            {
                isNew = false;
                return ref pool.Get(ent);
            }
            isNew = true;
            return ref pool.Add(ent);
        }

        public static ref T Ensure<T>(this EcsPool<T> pool, EcsUnsafeEntity ent)
            where T : struct
        {
            if (pool.Has(ent))
            {
                return ref pool.Get(ent);
            }
            return ref pool.Add(ent);
        }

    }

    public readonly struct EcsUnsafeEntity
    {
        public readonly int Id;

        public EcsUnsafeEntity(int id)
        {
            Id = id;
        }

        public static implicit operator int(in EcsUnsafeEntity ent)
        {
            return ent.Id;
        }

        public static explicit operator EcsUnsafeEntity(int id)
        {
            return new EcsUnsafeEntity(id);
        }

        public override string ToString() => Id.ToString();
    }
    //public readonly ref struct OptionalComponent<T> where T : struct
    //{
    //    private readonly EcsPool<T> _pool;
    //    private readonly int _ent;

    //    public OptionalComponent(EcsPool<T> pool, int ent)
    //    {
    //        _pool = pool;
    //        _ent = ent;
    //    }

    //    public ref T Ensure(out bool isNew)
    //    {
    //        if (_pool.Has(_ent))
    //        {
    //            isNew = false;
    //            return ref _pool.Get(_ent);
    //        }
    //        isNew = true;
    //        return ref _pool.Add(_ent);
    //    }

    //    public ref T Ensure()
    //    {
    //        if (_pool.Has(_ent))
    //        {
    //            return ref _pool.Get(_ent);
    //        }
    //        return ref _pool.Add(_ent);
    //    } 

    //    public bool Has() => _pool.Has(_ent);
    //}

    ////public readonly ref struct RequiredComponent<T> where T : struct
    ////{
    ////    private readonly EcsPool<T> _pool;
    ////    private readonly EcsUnsafeEntity _ent;

    ////    public RequiredComponent(EcsPool<T> pool, EcsUnsafeEntity ent)
    ////    {
    ////        _pool = pool;
    ////        _ent = ent;
    ////    }

    ////    public ref T Get() => ref _pool.Get(_ent);
    ////}

    //public readonly ref struct ExcludeComponent<T> where T : struct
    //{
    //    private readonly EcsPool<T> _pool;
    //    private readonly int _ent;

    //    public ExcludeComponent(EcsPool<T> pool, int ent)
    //    {
    //        _pool = pool;
    //        _ent = ent;
    //    }

    //    public ref T Add() => ref _pool.Add(_ent);
    //}
}
