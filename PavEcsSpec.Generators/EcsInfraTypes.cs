﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsSpec.Generators
{
    internal static class EcsInfraTypes
    {
        public static (string name, string code)[] Files = new (string name, string code)[]
        {
            new (nameof(ProviderTypes), ProviderTypes),
            new (nameof(EntityAttribute), EntityAttribute),
            new (nameof(AutoRegisterSystemAttribute), AutoRegisterSystemAttribute),
            new (nameof(TypeToWorldNameMap), TypeToWorldNameMap),
        };

        internal static void AddSources(GeneratorPostInitializationContext c)
        {
            foreach (var file in EcsInfraTypes.Files)
            {
                SourceText sourceText = SourceText.From(file.code.Trim(), Encoding.UTF8);
                c.AddSource(file.name, sourceText);
            }
        }

        private const string ProviderTypes = @"
using Leopotam.EcsLite;
using System;
using System.Runtime.CompilerServices;

namespace PavEcsSpec.Generated
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

        public bool Clear()
        {
            if (_pool.Has(_ent))
            {
                _pool.Del(_ent);
                return true;
            }
            return false;
        }

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

    public readonly ref struct RequiredComponent<T> where T : struct
    {
        private readonly EcsPool<T> _pool;
        private readonly int _ent;

        public RequiredComponent(EcsPool<T> pool, int ent)
        {
            _pool = pool;
            _ent = ent;
        }

        public void Remove() => _pool.Del(_ent);

        public ref T Get() => ref _pool.Get(_ent);
    }

    public readonly ref struct ExcludeComponent<T> where T : struct
    {
        private readonly EcsPool<T> _pool;
        private readonly int _ent;

        public ExcludeComponent(EcsPool<T> pool, int ent)
        {
            _pool = pool;
            _ent = ent;
        }

        public ref T Add() => ref _pool.Add(_ent);
    }

    //public interface IEntityProvider<T> where T : struct
    //{
    //    BaseEnumerator<T> GetEnumerator();
        
    //    EcsFilter Filter {get;}

    //    T Get(int ent);
    //}

    //public interface IEntityFactory<T> where T : struct
    //{
    //    T New();
    //    T? TryGet(Leopotam.EcsLite.EcsPackedEntityWithWorld entity);
    //    T? TryGetUnsafe(int entId);
    //}

    //public struct BaseEnumerator<T> : IDisposable where T : struct
    //{
    //    private EcsFilter.Enumerator _enumerator;
    //    private readonly IEntityProvider<T> _provider;

    //    public BaseEnumerator(EcsFilter.Enumerator enumerator, IEntityProvider<T> provider)
    //    {
    //        _enumerator = enumerator;
    //        _provider = provider;
    //    }

    //    public T Current
    //    {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get => _provider.Get(_enumerator.Current);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool MoveNext()
    //    {
    //        return _enumerator.MoveNext();
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void Dispose()
    //    {
    //        _enumerator.Dispose();
    //    }
    //}
}

";

        public const string EntityAttribute = @"
#nullable disable
using System;
namespace PavEcsSpec.Generated
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    sealed class EntityAttribute : Attribute
    {
        public EntityAttribute()
        {
        }
        
        public string Universe {get; set;}
        public bool SkipFilter {get; set;}

    }
}
";
        private const string AutoRegisterSystemAttribute = @"
using System;
namespace PavEcsSpec.Generated
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class AutoRegisterSystemAttribute : Attribute
    {
        public AutoRegisterSystemAttribute()
        {
        }
        //public string PropertyName { get; init; } 
    }
}
";

        public const string TypeToWorldNameMap = @"
#nullable disable
using System;
using System.Collections.Generic;

namespace PavEcsSpec.Generated
{
    public partial class TypeToWorldNameMap
    {
        public partial string GetWorldName<T>(string universeName) where T : struct;
        public string GetWorldName<T>() where T : struct => GetWorldName<T>(null);

    }
}
";
    }
}
