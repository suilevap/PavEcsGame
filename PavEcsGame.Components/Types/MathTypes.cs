using System;
using System.Runtime.CompilerServices;

namespace PavEcsGame.Components
{
    public struct Int2 : IEquatable<Int2>
    {
        public int X;
        public int Y;

        public static readonly Int2 Zero = new(0, 0);
        public static readonly Int2 One = new(1, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator +(in Int2 lhs, in Int2 rhs)
        {
            return new Int2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator -(in Int2 lhs, in Int2 rhs)
        {
            return new Int2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator *(in Int2 lhs, int rhs)
        {
            return new Int2(lhs.X * rhs, lhs.Y * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator *(int lhs, in Int2 rhs)
        {
            return new Int2(lhs * rhs.X, lhs * rhs.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator /(in Int2 lhs, int rhs)
        {
            return new Int2(lhs.X / rhs, lhs.Y / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Int2 lhs, in Int2 rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Int2 lhs, in Int2 rhs)
        {
            return lhs.X != rhs.X || lhs.Y != rhs.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Int2 other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Int2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }

    public struct Float3
    {
        public float X;
        public float Y;
        public float Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public struct Float4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}