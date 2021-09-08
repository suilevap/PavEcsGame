using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PavEcsGame.Components
{
    public struct FieldOfViewRequestEvent : IEquatable<FieldOfViewRequestEvent>
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

        public bool Equals(FieldOfViewRequestEvent other)
        {
            return Radius == other.Radius;
        }

        public override bool Equals(object obj)
        {
            return obj is FieldOfViewRequestEvent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Radius;
        }
    }
}
