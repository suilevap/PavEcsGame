using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Leopotam.Ecs;

namespace PavEcsGame.Components
{
    [DebuggerDisplay("{OtherEntity}")]
    struct TargetCollisionEventComponent
    {
        public EcsEntity OtherEntity;

        //public NewPositionComponent Position;
    }
}