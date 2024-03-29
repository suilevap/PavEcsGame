﻿using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PavEcsGame.Components
{
    [DebuggerDisplay("NextPos: {Value}")]
    public struct NewPositionComponent
    {
        public PositionComponent? Value;
    }
}
