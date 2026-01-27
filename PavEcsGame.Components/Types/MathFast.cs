using System;
using System.Runtime.CompilerServices;

namespace PavEcsGame.Components
{
    public static class MathFast
    {
        // ---- Constants ----
        public const float Epsilon = 1e-6f;
        public const float Deg2Rad = (float)(Math.PI / 180.0);
        public const float Rad2Deg = (float)(180.0 / Math.PI);

        // ---- Basic ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float x)
        {
            return x >= 0f ? x : -x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int x)
        {
            return x >= 0 ? x : -x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sign(float x)
        {
            return x >= 0f ? 1f : -1f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(int x)
        {
            return x >= 0 ? 1 : -1;
        }

        // ---- Clamp ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float v, float min, float max)
        {
            return v < min ? min : v > max ? max : v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int v, int min, int max)
        {
            return v < min ? min : v > max ? max : v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float v)
        {
            return v < 0f ? 0f : v > 1f ? 1f : v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Saturate(float v)
        {
            return Clamp01(v);
        }

        // ---- Lerp ----
        // Unclamped: t can be outside [0,1]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        // Clamped: t forced into [0,1]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t)
        {
            t = Clamp01(t);
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float a, float b, float v)
        {
            var d = b - a;
            if (Abs(d) <= Epsilon) return 0f;
            return Clamp01((v - a) / d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(float inMin, float inMax, float outMin, float outMax, float v)
        {
            var t = InverseLerp(inMin, inMax, v);
            return LerpUnclamped(outMin, outMax, t);
        }

        // ---- Smoothing ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothStep(float edge0, float edge1, float x)
        {
            var t = InverseLerp(edge0, edge1, x);
            return t * t * (3f - 2f * t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmootherStep(float edge0, float edge1, float x)
        {
            var t = InverseLerp(edge0, edge1, x);
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        // ---- Repeat / Wrap ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Repeat(float t, float length)
        {
            if (length <= 0f) return 0f;
            return t - Floor(t / length) * length;
        }

        // Wrap into [min, max)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Wrap(float v, float min, float max)
        {
            var len = max - min;
            if (len <= 0f) return min;
            return min + Repeat(v - min, len);
        }

        // ---- Approach / Move ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            var d = target - current;
            if (Abs(d) <= maxDelta) return target;
            return current + Sign(d) * maxDelta;
        }

        // ---- Angle helpers (degrees) ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaAngleDeg(float currentDeg, float targetDeg)
        {
            var d = Repeat(targetDeg - currentDeg, 360f);
            return d > 180f ? d - 360f : d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpAngleDeg(float aDeg, float bDeg, float t)
        {
            var d = DeltaAngleDeg(aDeg, bDeg);
            return aDeg + d * Clamp01(t);
        }

        // ---- Rounding / floor/ceil (fast wrappers) ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float x)
        {
            return (float)Math.Floor(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(float x)
        {
            return (float)Math.Ceiling(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float x)
        {
            return (float)Math.Round(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(float x)
        {
            return (int)Math.Floor(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(float x)
        {
            return (int)Math.Ceiling(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(float x)
        {
            return (int)Math.Round(x);
        }

        // ---- Misc ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b, float eps = 1e-5f)
        {
            return Abs(a - b) <= eps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Square(float x)
        {
            return x * x;
        }
    }
}