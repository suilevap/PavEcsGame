using Leopotam.EcsLite;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    internal static class UtilsSystemExtensions
    {
        public static IEcsSystems MarkPerf(this IEcsSystems systems, EcsUniverse universe, string tag)
        {
            return systems.Add(new PerfTimerSystem(universe, tag));
        }
    }
}
