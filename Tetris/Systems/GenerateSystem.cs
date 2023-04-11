using Leopotam.EcsLite;
using PavEcsSpec.EcsLite;

namespace Tetris.Systems;

internal class GenerateSystem<T> : IEcsRunSystem, IEcsSystemSpec
    where T: struct
{
    private readonly Func<T> _factoryMethod;
    private readonly EcsFilterSpec.Inc<EcsReadonlySpec<T>> _spec;

    private readonly EcsEntityFactorySpec<EcsSpec<T>> _factory;

    public GenerateSystem(EcsUniverse universe, Func<T> factoryMethod)
    {
        _factoryMethod = factoryMethod;
        universe.Register(this)
            .Build(ref _spec)
            .Build(ref _factory);
    }

    public void Run(IEcsSystems systems)
    {
        if (_spec.Filter.GetEntitiesCount() == 0)
        {
            _factory.NewUnsafeEntity()
                .Add(_factory.Pools,
                    _factoryMethod());
        }
    }
}