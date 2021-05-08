using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    [DebuggerDisplay("{Value}")]
    public struct PositionComponent
    {
        public Int2 Value;

        public PositionComponent(Int2 value)
        {
            Value = value;
        }
    }
}
