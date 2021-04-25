﻿using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;

namespace PavEcsGame.LeoEcsExtensions
{
    public static class LeoEcsExtensions
    {
        public static EcsSystems InjectByDeclaredType<T>(this EcsSystems systems,T dependency)
        {
            return systems.Inject(dependency, typeof(T));
        }

        public static EcsEntity Tag<T>(this EcsEntity ent) where T : struct, IEcsIgnoreInFilter
        {
            return ent.Replace(new T());
        }
    }
}
