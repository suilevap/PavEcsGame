using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Utils
{
    public static class Helper
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
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
            return data[(data.Length * rate / 256)];
        }
        public static char GetByRate(this string data, byte rate)
        {
            return data[(data.Length * rate / 256)];
        }

        public static void EnsureSize<T>(ref T[] result, int totalSize) where T : struct
        {
            if (result == null || result.Length < totalSize)
            {
                Array.Resize(ref result, totalSize);
            }
        }
    }
}
