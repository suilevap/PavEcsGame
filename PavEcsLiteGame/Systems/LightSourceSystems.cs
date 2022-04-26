using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class LightSourceSystems : IEcsRunSystem, IEcsSystemSpec
    {

        [Entity]
        private readonly partial struct LightEnt
        {
            public partial ref readonly LightSourceComponent Source();
            public partial OptionalComponent<FieldOfViewRequestEvent> FovRequest();
        }

        [Entity]
        private readonly partial struct PlayerEnt
        {
            public partial ref readonly PlayerIndexComponent PlayerIndex();
            public partial ref readonly VisualSensorComponent Sensor();
            public partial OptionalComponent<FieldOfViewRequestEvent> FovRequest();
        }

        public void Run(EcsSystems systems)
        {
            foreach(var ent in _providers.LightEntProvider)
            {
                var radius = ent.Source().Radius;
                ref var ev = ref ent.FovRequest().Ensure(out var isNew);
                if (isNew || ev.Radius < radius)
                {
                    ev.Radius = radius;
                }
            }

            foreach (var ent in _providers.PlayerEntProvider)
            {
                ent.FovRequest().Ensure().Radius = ent.Sensor().Radius;
            }

        }
    }
}
