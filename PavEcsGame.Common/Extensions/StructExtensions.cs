using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame
{
    public static class StructExtensions
    {
        public static bool TrySet<T>(this ref T? target, in T? value) where T : struct, IEquatable<T>
        {
            if ((!target.HasValue && value.HasValue) || !target.Equals(value))
            {
                target = value;
                return true;
            }

            return false;
        }

        public static bool TrySet<T>(this ref T target, in T value) where T : struct, IEquatable<T>
        {
            if (!target.Equals(value))
            {
                target = value;
                return true;
            }

            return false;
        }
    }
}
