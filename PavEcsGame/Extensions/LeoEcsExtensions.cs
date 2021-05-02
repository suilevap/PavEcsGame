using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;

namespace PavEcsGame.Extensions
{
    public static class LeoEcsExtensions
    {
        public static EcsSystems InjectByDeclaredType<T>(this EcsSystems systems, T dependency)
        {
            return systems.Inject(dependency, typeof(T));
        }

        public static EcsEntity Tag<T>(this EcsEntity ent) where T : struct, IEcsIgnoreInFilter
        {
            return ent.Replace(new T());
        }

        public static EcsEntity Tag<T>(this EcsEntity ent, bool set) where T : struct, IEcsIgnoreInFilter
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

    }
}
