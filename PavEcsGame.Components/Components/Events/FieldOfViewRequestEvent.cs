using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PavEcsGame.Components
{
    public struct FieldOfViewRequestEvent
    {
        public int Radius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in FieldOfViewRequestEvent lhs, in FieldOfViewRequestEvent rhs)
        {
            return lhs.Radius == rhs.Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in FieldOfViewRequestEvent lhs, in FieldOfViewRequestEvent rhs)
        {
            return lhs.Radius != rhs.Radius;
        }
    }
}
