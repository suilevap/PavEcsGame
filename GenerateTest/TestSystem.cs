using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using GenerateTest;
using Leopotam.Ecs.Types;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;


namespace PavEcsGame.Systems
{
    //[PavEcsSpec.Generators.AutoRegisterSystem]
    partial class TestSystem : IEcsRunSystem
    {
        //private readonly Entity.Provider _provider;
        //private readonly Entity2.Provider _provider2;
        private readonly IEntityProvider<Entity2> _provider2;
        //private readonly Entity2.Provider _provider2;



        public TestSystem(EcsSystems systems)
        {
            //_provider = new Entity.Provider(systems.GetWorld());
            _provider2 = new Entity2.Provider(systems.GetWorld());
             GetProvider(systems, ref _provider2);
        }

        private void GetProvider<T>(EcsSystems systems, ref IEntityProvider<T> result) where T : struct
        {

        }
        private void GetProvider(EcsSystems systems, ref IEntityProvider<Entity2> result)
        {
            result = new Entity2.Provider(systems.GetWorld());
        }


        //[PavEcsSpec.Generators.Entity]
        //private readonly ref partial struct Entity
        //{
        //    public partial ref readonly PositionComponent Pos();
        //    public partial ref readonly SpeedComponent Speed();
        //    public partial OptionalComponent<NewPositionComponent> NewPos();
        //    public partial OptionalComponent<PavEcsGame.Components.IsActiveTag> New2();
        //    public partial OptionalComponent<PavEcsGame.Components.IsActiveTag> New3();
        //    public partial ExcludeComponent<DestroyRequestTag> DestroyRequested();
        //}

        //[PavEcsSpec.Generators.Entity]
        private readonly partial struct Entity2
        {
            public partial ref PositionComponent Pos();
            public partial ref readonly SpeedComponent Speed();

            public static partial IEntityProvider<Entity2> GetProvider(EcsSystems systems);
            //public partial class Provider : IEntityProvider<Entity2> { }
        }


        //private readonly partial struct EntityEnumerator
        //{
        //    public partial Entity2 Current { get; }
        //}

        public void Run(EcsSystems systems)
        {
            //foreach (Entity entity in _provider)
            //{
            //    ref readonly var pos = ref entity.Pos();
            //    ref readonly var speed = ref entity.Speed();
            //    entity.NewPos().Ensure().Value = pos.Value + speed.Speed;
            //    Console.WriteLine($"entity. {entity.Pos().Value}");
            //}
            
            foreach (Entity2 entity in _provider2)
            {
                entity.Pos().Value += entity.Speed().Speed;
                //Console.WriteLine($"entity. {entity.Pos().Value}");
            }

        }
    }


    partial class TestSystem
    {
        private readonly partial struct Entity2
        {
            private readonly int _entityId;

            private readonly Provider _provider;

            private Entity2(int entityId, Provider provider)
            {
                _entityId = entityId;
                _provider = provider;
            }


            public partial ref Components.PositionComponent Pos()
            {
                return ref _provider._posPool.Get(_entityId);
            }

            public partial ref readonly Components.SpeedComponent Speed()
            {
                return ref _provider._speedPool.Get(_entityId);
            }

            public static partial IEntityProvider<Entity2> GetProvider(EcsSystems systems) 
                => new Provider(systems.GetWorld());


            public partial class Provider : IEntityProvider<Entity2>
            {
                public readonly EcsPool<Components.PositionComponent> _posPool;
                public readonly EcsPool<Components.SpeedComponent> _speedPool;


                private readonly EcsFilter _filter;

                public Provider(EcsWorld world)
                {
                    _posPool = world.GetPool<Components.PositionComponent>();
                    _speedPool = world.GetPool<Components.SpeedComponent>();

                    _filter = world.Filter<Components.PositionComponent>().Inc<Components.SpeedComponent>().End();
                }


                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Entity2 Get(int ent) => new Entity2(ent, this);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public BaseEnumerator<Entity2> GetEnumerator() => new BaseEnumerator<Entity2>(_filter.GetEnumerator(), this);


                //[MethodImpl(MethodImplOptions.AggressiveInlining)]
                //public Enumerator GetEnumerator()
                //{
                //    return new Enumerator(_filter.GetEnumerator(), this);
                //}

                //[MethodImpl(MethodImplOptions.AggressiveInlining)]
                //IEnumerator<Entity2> IEnumerable<Entity2>.GetEnumerator()
                //{
                //    return new Enumerator(_filter.GetEnumerator(), this);
                //}

                //IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();


                //public struct Enumerator : IEnumerator<Entity2>
                //{
                //    private EcsFilter.Enumerator _enumerator;
                //    private readonly Provider _provider;

                //    public Enumerator(EcsFilter.Enumerator enumerator, Provider provider)
                //    {
                //        _enumerator = enumerator;
                //        _provider = provider;
                //    }

                //    public Entity2 Current
                //    {
                //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
                //        get => new Entity2((EcsUnsafeEntity)_enumerator.Current, _provider);
                //    }


                //    object IEnumerator.Current => throw new NotImplementedException();

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

                //    public void Reset() => throw new NotImplementedException();
                //}
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
