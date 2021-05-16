using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    public struct PositionComponent
    {
        public Int2 Value;

        public PositionComponent(Int2 value)
        {
            Value = value;
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
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return Value.X ^ (Value.Y << 2);
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is PositionComponent))
            {
                return false;
            }
            var rhs = (PositionComponent)other;
            return Value.X == rhs.Value.X && Value.Y == rhs.Value.Y;
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
    }
}
