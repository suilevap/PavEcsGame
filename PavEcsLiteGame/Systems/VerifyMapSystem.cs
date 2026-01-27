using System.Diagnostics;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class VerifyMapSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        [Entity]
        private readonly partial struct Ent
        {
            public partial ref readonly PositionComponent Pos();
            public partial ref readonly ColliderComponent Collider();
        }

        [Entity]
        private readonly partial struct MapLoaded
        {
            public partial ref readonly MapLoadedEvent LoadEvent();
        }

        public VerifyMapSystem(EcsSystems universe, IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> map)
            : this(universe)
        {
            _map = map;
        }

        public void Run(IEcsSystems systems)
        {
            Debug.Assert(_providers.MapLoadedProvider.Filter.GetEntitiesCount() <= 1,
                "Too many",
                "{0} is expected to be no more than one per cycle", nameof(MapLoadedEvent));

            foreach (var ent in _providers.EntProvider)
            {
                ref readonly var pos = ref ent.Pos();
                var mapEnt = _map.Get(pos);

                Debug.Assert(mapEnt.EqualsTo(ent.Id), "Not stored entity", "Expected: {0}, Actual:{1}", ent.Id, mapEnt);
            }

            foreach (var (pos, ent) in _map.GetAll())
            {
                if (!ent.IsAlive())
                    continue;

                Debug.Assert(ent.Unpack(out var world, out int _) && world == _providers.EntProvider._world,
                    "Stored entity from different world", "ent: {0}", ent);

                Debug.Assert(_providers.EntProvider.TryGet(ent, out var entity),
                    "Stored ent without required components", "ent:{0}", ent);
            }
        }
    }
}