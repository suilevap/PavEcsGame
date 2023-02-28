using Leopotam.EcsLite;

namespace PavEcsSpec.EcsLite
{
    public static class LeoEcsExtensions
    {

        public static IEcsSystems UniDelHere<T>(this IEcsSystems system, EcsUniverse universe)
            where T : struct
        {
            return system.Add(new UniverseDelHereSystem<T>(universe));
        }

        public static IEcsSystems AddUniverse(this IEcsSystems system, out EcsUniverse universe)
        {
            universe = new EcsUniverse();
            return system.Add(universe);
        }


    }

}