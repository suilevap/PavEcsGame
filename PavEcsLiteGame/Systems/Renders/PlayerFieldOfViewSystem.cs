using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using System;
using System.Collections.Generic;
using System.Text;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    internal partial class PlayerFieldOfViewSystem : IEcsRunSystem, IEcsSystemSpec
    {

        [Entity]
        private partial struct PlayerFovEnt
        {
            public partial ref readonly AreaResultComponent<float> Fov();
            public partial ref readonly PlayerIndexComponent PlayerIndex();
            public partial ref readonly DirectionComponent Dir();
            public partial ref readonly PositionComponent Pos();
            public partial OptionalComponent<AreaResultComponent<VisibilityType>> Result();
        }

        public void Run(EcsSystems systems)
        {
            foreach(var ent in _providers.PlayerFovEntProvider)
            {
                ref readonly var fovComponent = ref ent.Fov();
                var filedOfView = fovComponent.Data;
                ref var result = ref ent.Result().Ensure(out var isNew);
                if (isNew)
                {
                    result.Data = new MapData<VisibilityType>();
                    result.Data.Init(filedOfView.MaxPos - filedOfView.MinPos);
                }
                if (fovComponent.Revision != result.Revision || isNew)//todo or direction changed?
                {
                    result.Revision = fovComponent.Revision;
                    var data = new EntityData(ent.Pos(), ent.Dir());

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
