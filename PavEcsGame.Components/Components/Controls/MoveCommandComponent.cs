using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    public struct MoveCommandComponent
    {
        public PositionComponent Target;

        public bool IsRelative;
    }
}
