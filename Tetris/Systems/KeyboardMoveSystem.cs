using Leopotam.EcsLite;
using PavEcsGame;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using Tetris.Components;

namespace Tetris.Systems;

class KeyboardMoveSystem : IEcsRunSystem, IEcsSystemSpec
{


    private readonly Dictionary<ConsoleKey, DirectionComponent> _moveConfigs;
    private readonly Dictionary<ConsoleKey, RotateComponent> _rotateConfigs;
    private readonly EcsFilterSpec
        .Inc<EcsReadonlySpec<Figure>>
        .Opt<EcsSpec<DirectionComponent, RotateComponent, OptionalCommandComponent>> _spec;

    public KeyboardMoveSystem(EcsUniverse universe)
    {
        universe
            .Register(this)
            .Build(ref _spec);
        
        _moveConfigs =
            new Dictionary<ConsoleKey, DirectionComponent>(){
                { ConsoleKey.UpArrow, new DirectionComponent(0, -1) },
                { ConsoleKey.DownArrow, new DirectionComponent(0, 1) },
                { ConsoleKey.LeftArrow, new DirectionComponent(-1, 0) },
                { ConsoleKey.RightArrow, new DirectionComponent(1, 0) }
            };
        _rotateConfigs =
            new Dictionary<ConsoleKey, RotateComponent>(){
                { ConsoleKey.Z, new RotateComponent(){RotateDirection = -1} },
                { ConsoleKey.X, new RotateComponent(){RotateDirection = 1} },
            };
    }

    public void Run(IEcsSystems systems)
    {
        ConsoleKey key = default;
        if (Console.KeyAvailable)
        {
            key = Console.ReadKey(true).Key;
        }

        if (_spec.Filter.IsEmpty())
            return;

        var (movePool, rotatePool, optCmdPool) = _spec.Optional;

        foreach (EcsUnsafeEntity ent in _spec.Filter)
        {
            var isOptionalCmd = false;
            if (_moveConfigs.TryGetValue(key, out var newSpeed))
            {
                movePool.Ensure(ent, out _) = newSpeed;
                isOptionalCmd = newSpeed.Direction.Y <= 0;
            }
            else
            {
                movePool.Del(ent);

                if (_rotateConfigs.TryGetValue(key, out var rotate))
                {
                    rotatePool.Ensure(ent, out _) = rotate;
                    isOptionalCmd = true;
                }
                else
                {
                    rotatePool.Del(ent);
                }
            }

            ent.TryTag(optCmdPool, isOptionalCmd);
        }
    }
}