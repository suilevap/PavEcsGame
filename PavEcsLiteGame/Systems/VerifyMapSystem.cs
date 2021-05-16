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

namespace PavEcsGame.Systems
{
    class VerifyMapSystem : IEcsRunSystem
    {
        private readonly IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> _map;

        private readonly EcsFilterSpec<EcsSpec<PositionComponent>, EcsSpec, EcsSpec> _spec;
        private readonly EcsFilterSpec<EcsSpec<MapLoadedEvent>, EcsSpec, EcsSpec> _mapLoadedEventSpec;

        public VerifyMapSystem(EcsUniverse universe, IReadOnlyMapData<PositionComponent, EcsPackedEntityWithWorld> map)
        {
            _map = map;
            _spec = universe
                .StartFilterSpec(
                    EcsSpec<PositionComponent>.Build())
                .End();

            _mapLoadedEventSpec = universe
                .StartFilterSpec(
                    EcsSpec<MapLoadedEvent>.Build())
                .End();
        }

        public void Run(EcsSystems systems)
        {
            Debug.Assert(_mapLoadedEventSpec.Filter.GetEntitiesCount() <= 1, 
                $"{nameof(MapLoadedEvent)} is expected to be no more than one per cycle");


            var posPool = _spec.Include.Pool1;
            foreach(EcsUnsafeEntity ent in _spec.Filter)
            {
                ref var pos = ref posPool.Get(ent);
                var mapEnt = _map.Get(pos);

                Debug.Assert(mapEnt.IsSame(ent), $"Not stored entity: Expected: {ent}, Actual:{mapEnt}");
            }

            foreach(var (pos,ent) in _map.GetAll())
            {
                if (!ent.IsAlive())
                    continue;
                
                Debug.Assert(ent.IsBelongTo(_spec), $"Stored entity from different world: {ent}");

                if (ent.Unpack(out _, out EcsUnsafeEntity entId))
                {
                    Debug.Assert(posPool.Has(entId), $"Stored ent without pos: {ent}");
                    var entPos = posPool.Get(entId);
                    Debug.Assert(pos.Equals(entPos), $"Stored in wrong place: {ent}, actual pos:{pos}, expected: {entPos}");
                }
            }
        }
    }
}
