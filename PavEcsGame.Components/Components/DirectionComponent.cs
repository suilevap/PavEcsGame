using Leopotam.Ecs.Types;
using System;

namespace PavEcsGame.Components
{
    public struct DirectionComponent
    {
        public Int2 Direction; //todo use byte
    }

    public enum Direction
    {
        None = 0,
        Right = 1,
        Up = 2,
        Left = 3,
        Down = 4
    }

    public static class DirectionExtension
    {
        public static Direction ToDirection(this in Int2 dir)
        {
            if (dir == Int2.Zero)
                return Direction.None;

            Direction result;
            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
            {
                if (dir.X > 0)
                {
                    result = Direction.Right;
                }
                else
                {
                    result = Direction.Left;
                }
            }
            else
            {
                if (dir.Y > 0)
                {
                    result = Direction.Up;
                }
                else
                {
                    result = Direction.Down;
                }
            }
            return result;
        }
    }
}
