using System;
using System.Collections.Generic;
using System.Text;

namespace PaveEcsGame.Utils
{
    public static class Helper
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        public static T GetByRate<T>(this T[] data, float rate)
        {
            return data[(int)(data.Length * rate - 0.5f)];
        }

        public static T GetByRate<T>(this T[] data, byte rate)
        {
            return data[(data.Length * rate / 256)];
        }
    }
}
