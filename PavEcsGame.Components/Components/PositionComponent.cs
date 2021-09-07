using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    public struct PositionComponent : IEquatable<PositionComponent>
    {
        public Int2 Value;

        public PositionComponent(Int2 value)
        {
            Value = value;
        }
        public PositionComponent(int x, int y)
        {
            Value = new Int2(x,y);
        }

        public override string ToString() => $"Pos:{Value}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator +(in PositionComponent lhs, in PositionComponent rhs)
        {
            PositionComponent res;
            res.Value.X = lhs.Value.X + rhs.Value.X;
            res.Value.Y = lhs.Value.Y + rhs.Value.Y;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator -(in PositionComponent lhs, in PositionComponent rhs)
        {
            PositionComponent res;
            res.Value.X = lhs.Value.X - rhs.Value.X;
            res.Value.Y = lhs.Value.Y - rhs.Value.Y;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator *(in PositionComponent lhs, in PositionComponent rhs)
        {
            PositionComponent res;
            res.Value.X = lhs.Value.X * rhs.Value.X;
            res.Value.Y = lhs.Value.Y * rhs.Value.Y;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator *(in PositionComponent lhs, int rhs)
        {
            PositionComponent res;
            res.Value.X = lhs.Value.X * rhs;
            res.Value.Y = lhs.Value.Y * rhs;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator /(in PositionComponent lhs, int rhs)
        {
            PositionComponent res;
            res.Value.X = lhs.Value.X / rhs;
            res.Value.Y = lhs.Value.Y / rhs;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator -(in PositionComponent lhs)
        {
            PositionComponent res;
            res.Value.X = -lhs.Value.X;
            res.Value.Y = -lhs.Value.Y;
            return res;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ScalarMul(in PositionComponent lhs, in PositionComponent rhs)
        {
            return lhs.Value.X * rhs.Value.X + lhs.Value.Y * rhs.Value.Y;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in PositionComponent lhs, in PositionComponent rhs)
        {
            return lhs.Value.X == rhs.Value.X && lhs.Value.Y == rhs.Value.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in PositionComponent lhs, in PositionComponent rhs)
        {
            return lhs.Value.X != rhs.Value.X || lhs.Value.Y != rhs.Value.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Int2(in PositionComponent v)
        {
            return v.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator PositionComponent(Int2 v)
        {
            return new PositionComponent(v);
        }

        bool IEquatable<PositionComponent>.Equals(PositionComponent other)
        {
            return Value.Equals(other.Value);
        }
    }
}
