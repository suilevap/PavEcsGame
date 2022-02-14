﻿using System;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    [PavEcsSpec.Generated.AutoRegisterSystem]
    partial class EmptySystem : IEcsRunSystem
    {
        private readonly IEntityProvider<Entity> _provider;
        private readonly IEntityProvider<Entity2> _provider2;

        public EmptySystem(EcsSystems systems)
        {
            _provider = Entity.GetProvider(systems);
            _provider2 = Entity2.GetProvider(systems);
        }

        //private partial EmptySystem(EcsSystems systems);

        [PavEcsSpec.Generated.Entity]
        private readonly partial struct Entity
        {
            public partial ref readonly PavEcsGame.Components.PositionComponent Pos();
            public partial ref readonly SpeedComponent Speed();
            public partial OptionalComponent<NewPositionComponent> NewPos();
            public partial OptionalComponent<PavEcsGame.Components.IsActiveTag> New2();
            //public partial OptionalComponent<PavEcsGame.Components.IsActiveTag> New3();
            public partial ExcludeComponent<DestroyRequestTag> DestroyRequested();

            public static partial IEntityProvider<Entity> GetProvider(EcsSystems systems);
        }

        [PavEcsSpec.Generated.Entity]
        private readonly partial struct Entity2
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly SpeedComponent Speed();
            public static partial IEntityProvider<Entity2> GetProvider(EcsSystems systems);

        }


        public void Run(EcsSystems systems)
        {
            foreach (Entity entity in _provider)
            {
                ref readonly var pos = ref entity.Pos();
                ref readonly var speed = ref entity.Speed();
                entity.NewPos().Ensure().Value = pos.Value + speed.Speed;
                Console.WriteLine($"entity. {entity.Pos().Value}");
            }

            foreach (Entity2 entity in _provider2)
            {
                Console.WriteLine($"entity. {entity.Pos().Value}");
            }

        }
    }

    //partial class EmptySystem
    //{

    //    public readonly ref partial struct Entity
    //    {
    //        private readonly EcsUnsafeEntity _ent;

    //        private readonly Provider _provider;

    //        private Entity(EcsUnsafeEntity ent, Provider provider)
    //        {
    //            _ent = ent;
    //            _provider = provider;
    //        }

    //        public partial ref readonly PositionComponent Pos()
    //        {
    //            return ref _provider._posPool.Get(_ent);
    //        }

    //        public partial OptionalComponent<SpeedComponent> Speed()
    //        {
    //            return new OptionalComponent<SpeedComponent>(_provider._speedPool, _ent);
    //        }

    //        public partial class Provider
    //        {
    //            public readonly EcsPool<PositionComponent> _posPool;
    //            public readonly EcsPool<SpeedComponent> _speedPool;
    //            private readonly EcsFilter _filter;

    //            public Provider(EcsWorld world)
    //            {
    //                _posPool = world.GetPool<PositionComponent>();
    //                _speedPool = world.GetPool<SpeedComponent>();
    //                _filter = world.Filter<PositionComponent>().End();
    //            }


    //            [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //            public Enumerator GetEnumerator()
    //            {
    //                return new Enumerator(_filter.GetEnumerator(), this);
    //            }

    //            public struct Enumerator : IDisposable
    //            {
    //                private EcsFilter.Enumerator _enumerator;
    //                private readonly Provider _provider;

    //                public Enumerator(EcsFilter.Enumerator enumerator, Provider provider)
    //                {
    //                    _enumerator = enumerator;
    //                    _provider = provider;
    //                }

    //                public Entity Current
    //                {
    //                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //                    get => new Entity((EcsUnsafeEntity)_enumerator.Current, _provider);
    //                }

    //                [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //                public bool MoveNext()
    //                {
    //                    return _enumerator.MoveNext();
    //                }

    //                [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //                public void Dispose()
    //                {
    //                    _enumerator.Dispose();
    //                }
    //            }

    //        }

    //    }

    //    private readonly Entity.Provider _provider;
    //}

   
}