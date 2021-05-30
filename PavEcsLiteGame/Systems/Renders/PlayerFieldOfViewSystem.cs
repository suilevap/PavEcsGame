using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PaveEcsGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Systems.Renders
{
    internal class PlayerFieldOfViewSystem : IEcsRunSystem
    {

        private readonly EcsFilterSpec<
            EcsSpec<AreaResultComponent<float>, PlayerIndexComponent>,
            EcsSpec<AreaResultComponent<VisibilityType>>,
            EcsSpec> _playerFieldOfViewSpec;

        public PlayerFieldOfViewSystem(EcsUniverse universe)
        {
            universe
                .Build(ref _playerFieldOfViewSpec);
        }
        public void Run(EcsSystems systems)
        {
            foreach (EcsUnsafeEntity ent in _playerFieldOfViewSpec.Filter)
            {
                ref var fovComponent = ref _playerFieldOfViewSpec.Include.Pool1.Get(ent);
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
