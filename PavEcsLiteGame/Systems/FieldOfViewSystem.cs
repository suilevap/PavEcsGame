using System;
using Leopotam.EcsLite;
using PavEcsGame.Area;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class FieldOfViewSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly FieldOfViewComputationInt2 _fieldOfView;
        private readonly Func<Int2, Int2, bool> _hasObstacles;

        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;
        private (Int2 delta, float value)[]? _fieldOfViewResult;

        [Entity]
        private readonly partial struct FovSourceEnt
        {
            public struct FieldOfViewCalculated
            {
                public PositionComponent Position;
                public FieldOfViewRequestEvent Request;
            }

            public partial ref readonly PositionComponent Pos();
            public partial RequiredComponent<FieldOfViewRequestEvent> Request();

            public partial OptionalComponent<AreaResultComponent<float>> Result();
            public partial OptionalComponent<FieldOfViewCalculated> FovCalculated();
        }

        [Entity(SkipFilter = true)]
        private partial struct ObstacleEnt
        {
            public partial ref readonly PositionComponent Pos();
            public partial ExcludeComponent<SpeedComponent> Speed();
        }

        public FieldOfViewSystem(
            EcsSystems universe,
            IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> map)
            : this(universe)
        {
            _map = map;

            _fieldOfView = new FieldOfViewComputationInt2();
            _hasObstacles = HasObstacle;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in _providers.FovSourceEntProvider)
            {
                ref readonly var pos = ref ent.Pos();
                ref var request = ref ent.Request().Get();
                ref var prevInput = ref ent.FovCalculated().Ensure(out var isNew);
                if (isNew || prevInput.Position != pos || prevInput.Request != request)
                {
                    var radius = request.Radius;
                    prevInput.Position = pos;
                    prevInput.Request = request;
                    UpdateFieldOfViewData(ent, pos, radius);
                }

                ent.Request().Remove();
            }

            void UpdateFieldOfViewData(in FovSourceEnt ent, in PositionComponent pos, int radius)
            {
                ref var result = ref ent.Result().Ensure(out var isNew);
                if (isNew)
                {
                    result.Data = new MapData<float>();
                    result.Data.Init(_map.MaxPos - _map.MinPos);
                }
                else
                {
                    result.Data.Clear();
                    result.Revision++;
                }

                var radiusSq = radius * radius;
                _fieldOfView.Compute(pos.Value, radius, _hasObstacles, ref _fieldOfViewResult, out var count);
                for (var i = 0; i < count; i++)
                {
                    ref var item = ref _fieldOfViewResult[i];
                    var p = pos.Value + item.delta;
                    if (_map.IsValid(p))
                    {
                        var sqD = pos.Value.DistanceSquare(in p);
                        if (sqD <= radiusSq)
                        {
                            var lightValue = item.value; // * (1 - sqD / radiusSq);
                            ref var v = ref result.Data.GetRef(result.Data.GetSafePos(new PositionComponent(p)));
                            v += lightValue;
                        }
                    }
                }
            }
        }

        private bool HasObstacle(Int2 pos, Int2 delta)
        {
            var p = pos + delta;
            if (!_map.IsValid(p))
                return true;
            var ent = _map.Get(p);
            return _providers.ObstacleEntProvider.TryGet(ent).HasValue;
        }
    }
}