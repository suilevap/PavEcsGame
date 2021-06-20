using System;
using System.Diagnostics;
using Leopotam.Ecs.Types;

namespace PaveEcsGame
{
    public static class Int2Extensions
    {
        public static bool IsInRange(this in Int2 pos, in Int2 target, int radius)
        {
            var diff = target - pos;
            return (diff.X * diff.X + diff.Y * diff.Y) <= radius * radius;
        }
        public static int DistanceSquare(this in Int2 pos, in Int2 target)
        {
            var diff = target - pos;
            return (diff.X * diff.X + diff.Y * diff.Y);
        }

        public static int LengthSqure(this in Int2 diff)
        {
            return (diff.X * diff.X + diff.Y * diff.Y);
        }

        ////v.X v.Y 1 0
        //// v.X * x + v.Y * y  V.y * x -V.x*y

        //public static Int2 RotateLeft90(this in Int2 v)
        //{
        //    return new Int2(-v.Y, v.X); //0 -1
        //}

        //public static Int2 Rotate180(this in Int2 v)
        //{
        //    return new Int2(-v.X, -v.Y); //-1 0
        //}

        //public static Int2 RotateRight90(this in Int2 v)
        //{
        //    return new Int2(v.Y, -v.X); //0 1
        //}

        public static Int2 Rotate(this in Int2 v, in Int2 dir)
        {
            if (dir == Int2.Zero)
                return v;
            Debug.Assert(dir.LengthSqure() == 1, "Only normilized vector is supproted");
            return new Int2(v.X * dir.X + v.Y * dir.Y, - v.Y * dir.X + v.X * dir.Y);
        }
    }
}
