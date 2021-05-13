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
    }
}
