using System;
using PavEcsGame.Components;

namespace PavEcsGame.Utils
{
    public static class Helper
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }

        public static T GetRandom<T>(this T[] data, Random rnd)
        {
            return data[rnd.Next(data.Length)];
        }

        public static T GetByRate<T>(this T[] data, float rate)
        {
            return data[(int)(data.Length * rate - 0.5f)];
        }

        public static T GetByRate<T>(this T[] data, byte rate)
        {
            return data[data.Length * rate / 256];
        }

        public static char GetByRate(this string data, byte rate)
        {
            return data[data.Length * rate / 256];
        }

        /// <summary>
        /// Gets a color from a gradient array using linear interpolation for smooth transitions.
        /// Maps intensity [0-255] to gradient array indices and interpolates between keyframes.
        /// </summary>
        /// <param name="gradient">Gradient color array (keyframe colors)</param>
        /// <param name="intensity">Intensity value [0-255]</param>
        /// <returns>Interpolated color from the gradient</returns>
        public static Color GetByRateLerp(this Color[] gradient, byte intensity)
        {
            if (intensity == 0) return gradient[0];
            if (intensity == 255) return gradient[^1];

            // Map intensity to fractional index in gradient array
            float exactIndex = (gradient.Length - 1) * intensity / 255f;
            int lowerIndex = (int)exactIndex;
            int upperIndex = lowerIndex + 1;
            float t = exactIndex - lowerIndex; // Lerp factor [0-1]

            return Color.Lerp(gradient[lowerIndex], gradient[upperIndex], t);
        }

        public static void EnsureSize<T>(ref T[] result, int totalSize) where T : struct
        {
            if (result == null || result.Length < totalSize) Array.Resize(ref result, totalSize);
        }
    }
}