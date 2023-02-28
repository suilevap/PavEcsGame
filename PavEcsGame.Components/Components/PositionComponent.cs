using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    public readonly struct PositionComponent : IEquatable<PositionComponent>
    {

        public readonly Int2 Value;

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
            var x = lhs.Value.X + rhs.Value.X;
            var y = lhs.Value.Y + rhs.Value.Y;
            return new PositionComponent(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator -(in PositionComponent lhs, in PositionComponent rhs)
        {
            var x = lhs.Value.X - rhs.Value.X;
            var y = lhs.Value.Y - rhs.Value.Y;
            return new PositionComponent(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator *(in PositionComponent lhs, in PositionComponent rhs)
        {
            var x = lhs.Value.X * rhs.Value.X;
            var y = lhs.Value.Y * rhs.Value.Y;
            return new PositionComponent(x,y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator *(in PositionComponent lhs, int rhs)
        {
            var x = lhs.Value.X * rhs;
            var y = lhs.Value.Y * rhs;
            return new PositionComponent(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator /(in PositionComponent lhs, int rhs)
        {
            var x = lhs.Value.X / rhs;
            var y = lhs.Value.Y / rhs;
            return new PositionComponent(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositionComponent operator -(in PositionComponent lhs)
        {
            var x = -lhs.Value.X;
            var y = -lhs.Value.Y;
            return new PositionComponent(x, y);
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

        public override bool Equals(object obj)
        {
            return obj is PositionComponent other && Equals(other);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(PositionComponent other)
        {
            return Value.Equals(other.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PositionComponent Add(int x, int y)
        {
            return new PositionComponent(Value.X + x, Value.Y + y);
        }
    }
}
