using Leopotam.Ecs;
using PavEcsGame.Components;

namespace PavEcsGame.Extensions
{
    public static class LeoEcsExtensions
    {
        public static EcsSystems InjectByDeclaredType<T>(this EcsSystems systems, T dependency)
        {
            return systems.Inject(dependency, typeof(T));
        }

        public static EcsEntity Tag<T>(this EcsEntity ent) where T : struct, ITag//, IEcsIgnoreInFilter
        {
            return ent.Replace(new T());
        }

        public static EcsEntity Tag<T>(this EcsEntity ent, bool set) where T : struct, ITag//, IEcsIgnoreInFilter
        {
            if (set)
            {
                return ent.Replace(new T());
            }
            else
            {
                ent.Del<T>();
                return ent;
            }
        }

        // public static bool TryGet<T>(this EcsEntity ent, ref T component) where T : struct
        // {
        //     if (ent.Has<T>())
        //     {
        //         component = ref ent.Get<T>();
        //         return true;
        //     }
        //     return false;
        // }

    }
}
