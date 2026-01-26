#nullable disable
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;

//namespace PavEcsSpec.Generated
//{
//    public partial class TypeToWorldName
//    {
//        public partial string GetWorldName<T>(string universeName) where T : struct;
//        public string GetWorldName<T>() where T : struct => GetWorldName<T>(null);

//    }
//}

namespace PavEcsSpec.Generated2
{
    public readonly ref struct OptionalComponent<T> where T : struct
    {
        private readonly EcsPool<T> _pool;
        private readonly int _ent;

        public OptionalComponent(EcsPool<T> pool, int ent)
        {
            _pool = pool;
            _ent = ent;
        }
        public void Remove() => _pool.Del(_ent);

        public ref T Ensure(out bool isNew)
        {
            if (_pool.Has(_ent))
            {
                isNew = false;
                return ref _pool.Get(_ent);
            }
            isNew = true;
            return ref _pool.Add(_ent);
        }

        public ref T Ensure()
        {
            if (_pool.Has(_ent))
            {
                return ref _pool.Get(_ent);
            }
            return ref _pool.Add(_ent);
        }

        public bool Has() => _pool.Has(_ent);
    }

    public readonly ref struct RequredComponent<T> where T : struct
    {
        private readonly EcsPool<T> _pool;
        private readonly int _ent;

        public RequredComponent(EcsPool<T> pool, int ent)
        {
            _pool = pool;
            _ent = ent;
        }
        public void Remove() => _pool.Del(_ent);


        public ref T Get() => ref _pool.Get(_ent);
    }
    //public partial class TypeToWorldName
    //{
    //    public partial string GetWorldName<T>(string universeName) where T : struct
    //    {
    //        return GetMap(universeName).GetWorldName<T>();
    //    }

    //    private static TypeToWorldName GetMap(string universeName)
    //    {
    //        if (string.IsNullOrEmpty(universeName))
    //            return Default;
    //        switch(universeName)
    //        {
    //            case "Uinverse123":
    //                return Uinverse123;

    //            default:
    //                throw new ArgumentException($"Unknown universe {universeName}");
    //        }
    //    }

    //    public static TypeToWorldName Default = new TypeToWorldName()
    //    {
    //        _map = new Dictionary<Type, string>()
    //        {
    //            { typeof(PavEcsGame.Components.PositionComponent),"world1" },
    //        }
    //    };

    //    public static TypeToWorldName Uinverse123 = new TypeToWorldName()
    //    {
    //        _map = new Dictionary<Type, string>()
    //        {
    //            { typeof(PavEcsGame.Components.PositionComponent),"world1" },
    //        }
    //    };

    //    private Dictionary<Type, string> _map;

    //    public string GetWorld<T>() where T : struct => _map[typeof(T)];

    //}
}

//using Leopotam.EcsLite;
//using System;
//using System.Runtime.CompilerServices;

//namespace PavEcsSpec.Generated
//{
//    public interface IEntityProvider<T> where T : struct
//    {
//        BaseEnumerator<T> GetEnumerator();

//        T Get(int ent);
//    }
//    public struct BaseEnumerator<T> : IDisposable where T : struct
//    {
//        private EcsFilter.Enumerator _enumerator;
//        private readonly IEntityProvider<T> _provider;

//        public BaseEnumerator(EcsFilter.Enumerator enumerator, IEntityProvider<T> provider)
//        {
//            _enumerator = enumerator;
//            _provider = provider;
//        }

//        public T Current
//        {
//            [MethodImpl(MethodImplOptions.AggressiveInlining)]
//            get => _provider.Get(_enumerator.Current);
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public bool MoveNext()
//        {
//            return _enumerator.MoveNext();
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public void Dispose()
//        {
//            _enumerator.Dispose();
//        }
//    }
//}


