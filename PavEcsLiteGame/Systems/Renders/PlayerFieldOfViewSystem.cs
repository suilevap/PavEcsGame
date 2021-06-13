using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PaveEcsGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Systems.Renders
{
    internal class PlayerFieldOfViewSystem : IEcsRunSystem, IEcsSystemSpec
    {

        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<AreaResultComponent<float>, PlayerIndexComponent>>
            .Opt<EcsSpec<AreaResultComponent<VisibilityType>>> _playerFieldOfViewSpec;

        public PlayerFieldOfViewSystem(EcsUniverse universe)
        {
            universe
                .Register(this)
                .Build(ref _playerFieldOfViewSpec);
        }
        public void Run(EcsSystems systems)
        {
            foreach (EcsUnsafeEntity ent in _playerFieldOfViewSpec.Filter)
            {
                ref readonly var fovComponent = ref _playerFieldOfViewSpec.Include.Pool1.Get(ent);
                var filedOfView = fovComponent.Data;
                ref var result = ref _playerFieldOfViewSpec.Optional.Pool1.Ensure(ent, out var isNew);
                if (isNew)
                {
                    result.Data = new MapData<VisibilityType>();
                    result.Data.Init(filedOfView.MaxPos - filedOfView.MinPos);
                }
                if (fovComponent.Revision != result.Revision || isNew)
                {
                    result.Revision = fovComponent.Revision;
                    result.Data.Merge(filedOfView, this, VisibilityMerge);
                }
            }
        }
        
        private static void VisibilityMerge(
            in PlayerFieldOfViewSystem _,
            in PositionComponent pos,
            ref VisibilityType sourceValue, 
            in float targetValue)
        {
            if (targetValue > 0.1)
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
