using System;
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
    }
}
