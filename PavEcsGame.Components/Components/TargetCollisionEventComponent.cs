using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Leopotam.Ecs;

namespace PavEcsGame.Components
{
    [DebuggerDisplay("{OtherEntity}")]
    public struct TargetCollisionEventComponent<T> where T : struct
    {
        public T OtherEntity;

        //public NewPositionComponent Position;
    }
}