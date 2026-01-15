using System.Runtime.CompilerServices;

namespace PavEcsGame.Components
{
    public struct Int2
    {
        public int X;
        public int Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool Equals(in Int2 a, in Int2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator ==(in Int2 a, in Int2 b)
        {
            return Int2.Equals(a,b);
        }

        public static bool operator !=(in Int2 a, in Int2 b)
        {
            return !Int2.Equals(a,b);
        }


        public override int GetHashCode()
        {
            return X^Y;
        }


        public override bool Equals(object obj)
        {
            return ((obj is Int2 b) && Int2.Equals(this, b));
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public static Int2 Zero => new Int2(0, 0);
    public static Int2 One  => new Int2(1, 1);

    // -------------------------
    // Operators (component-wise)
    // -------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator +(Int2 a, Int2 b)
        => new Int2(a.X + b.X, a.Y + b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator -(Int2 a, Int2 b)
        => new Int2(a.X - b.X, a.Y - b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator -(Int2 v)
        => new Int2(-v.X, -v.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator *(Int2 a, Int2 b)
        => new Int2(a.X * b.X, a.Y * b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator /(Int2 a, Int2 b)
        => new Int2(a.X / b.X, a.Y / b.Y);

    // -------------------------
    // Scalar ops
    // -------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator *(Int2 v, int s)
        => new Int2(v.X * s, v.Y * s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator *(int s, Int2 v)
        => new Int2(v.X * s, v.Y * s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator /(Int2 v, int s)
        => new Int2(v.X / s, v.Y / s);
    }

    public struct Float3
    {
        public float X;
        public float Y;
        public float Z;

        public Float3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool Equals(in Float3 a, in Float3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator ==(in Float3 a, in Float3 b)
        {
            return Int2.Equals(a,b);
        }

        public static bool operator !=(in Float3 a, in Float3 b)
        {
            return !Int2.Equals(a,b);
        }


        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            return ((obj is Float3) && this.Equals((Float3)obj));
        }

        public override string ToString()
        {
            return $"({X},{Y}, {Z})";
        }

        public static Float3 Zero => new Float3(0f, 0f, 0f);
        public static Float3 One  => new Float3(1f, 1f, 1f);

        // -------------------------
        // Operators (component-wise)
        // -------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator +(Float3 a, Float3 b)
            => new Float3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator -(Float3 a, Float3 b)
            => new Float3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator -(Float3 v)
            => new Float3(-v.X, -v.Y, -v.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(Float3 a, Float3 b)
            => new Float3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator /(Float3 a, Float3 b)
            => new Float3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        // -------------------------
        // Scalar ops
        // -------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(Float3 v, float s)
            => new Float3(v.X * s, v.Y * s, v.Z * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(float s, Float3 v)
            => new Float3(v.X * s, v.Y * s, v.Z * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator /(Float3 v, float s)
            => new Float3(v.X / s, v.Y / s, v.Z / s);

    }



}
