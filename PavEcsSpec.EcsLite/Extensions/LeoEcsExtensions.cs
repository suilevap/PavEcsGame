using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public static class LeoEcsExtensions
    {

        public static EcsSystems UniDelHere<T>(this EcsSystems system, EcsUniverse universe)
            where T : struct
        {
            return system.Add(new UniverseDelHereSystem<T>(universe));
        }

        public static EcsSystems AddUniverse(this EcsSystems system, out EcsUniverse universe)
        {
            universe = new EcsUniverse();
            return system.Add(universe);
        }


    }

}