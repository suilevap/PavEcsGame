using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Systems.Renders
{
    internal class PlayerFieldOfViewSystem : IEcsRunSystem, IEcsSystemSpec
    {

        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<AreaResultComponent<float>, PlayerIndexComponent, DirectionComponent, PositionComponent>>
            .Opt<EcsSpec<AreaResultComponent<VisibilityType>>> _playerFieldOfViewSpec;

        public PlayerFieldOfViewSystem(EcsUniverse universe)
        {
            universe
                .Register(this)
                .Build(ref _playerFieldOfViewSpec);
        }
        public void Run(IEcsSystems systems)
        {
            var (fovPool, _, dirPool, posPool) = _playerFieldOfViewSpec.Include;
            var resultPool = _playerFieldOfViewSpec.Optional.Pool1;
            foreach (EcsUnsafeEntity ent in _playerFieldOfViewSpec.Filter)
            {
                ref readonly var fovComponent = ref fovPool.Get(ent);
                var filedOfView = fovComponent.Data;
                ref var result = ref resultPool.Ensure(ent, out var isNew);
                if (isNew)
                {
                    result.Data = new MapData<VisibilityType>();
                    result.Data.Init(filedOfView.MaxPos - filedOfView.MinPos);
                }
                if (fovComponent.Revision != result.Revision || isNew)//todo or direction changed?
                {
                    result.Revision = fovComponent.Revision;
                    var data = new EntityData(posPool.Get(ent), dirPool.Get(ent));

                    result.Data.Merge(filedOfView, data, VisibilityMerge);
                }
            }
        }
        private readonly struct EntityData
        {
            public readonly DirectionComponent Direction;
            public readonly PositionComponent Position;

            public EntityData(PositionComponent position, DirectionComponent direction) : this()
            {
                Position = position;
                Direction = direction;
            }
        }

        private static void VisibilityMerge(
            in EntityData data,
            in PositionComponent pos,
            ref VisibilityType sourceValue, 
            in float targetValue)
        {
            if (targetValue > 0.1 
                )//&& PositionComponent.ScalarMul(pos - data.Position, data.Direction.Direction) >= 0)//todo proper angle check
            {
                sourceValue |= VisibilityType.Visible | VisibilityType.Known;
            }
            else
            {
                sourceValue &= ~VisibilityType.Visible;
            }
        }
    }
}
