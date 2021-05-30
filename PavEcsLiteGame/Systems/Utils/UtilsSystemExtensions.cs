using Leopotam.EcsLite;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    internal static class UtilsSystemExtensions
    {
        public static EcsSystems MarkPerf(this EcsSystems systems, EcsUniverse universe, string tag)
        {
            return systems.Add(new PerfTimerSystem(universe, tag));
        }
    }
}
