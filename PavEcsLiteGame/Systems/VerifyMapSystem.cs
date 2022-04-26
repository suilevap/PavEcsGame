using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Leopotam.EcsLite;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    partial class VerifyMapSystem : IEcsRunSystem, IEcsSystemSpec
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

        public void Run(EcsSystems systems)
        {
            Debug.Assert(_providers.MapLoadedProvider.Filter.GetEntitiesCount() <= 1,
               $"{nameof(MapLoadedEvent)} is expected to be no more than one per cycle");

            foreach(var ent in _providers.EntProvider)
            {
                ref readonly var pos = ref ent.Pos();
                var mapEnt = _map.Get(pos);

                Debug.Assert(mapEnt.EqualsTo(ent.Id), $"Not stored entity: Expected: {ent.Id}, Actual:{mapEnt}");
            }
            foreach(var (pos,ent) in _map.GetAll())
            {
                if (!ent.IsAlive())
                    continue;
                
                Debug.Assert(ent.Unpack(out var world, out int _) && world == _providers.EntProvider._world, 
                    $"Stored entity from different world: {ent}");

                Debug.Assert(_providers.EntProvider.TryGet(ent, out var entity), $"Stored ent without required components: {ent}");
            }
        }

    }
}
