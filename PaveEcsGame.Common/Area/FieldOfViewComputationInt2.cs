using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs.Types;

namespace PaveEcsGame.Area
{
    public class FieldOfViewComputationInt2 : FieldOfViewComputation<Int2>
    {
        protected override (IEnumerable<Int2> points, int count) GetCircle(Int2 pos, int radius)
        {
            return (GetCirclePoints(pos, radius), GetCircleCount(radius));
        }

        private static int GetCircleCount(int radius)
        {
            return radius* 2 * 4;
        }

        private static IEnumerable<Int2> GetCirclePoints(Int2 pos, int radius)
        {
            for (int i = -radius; i < radius; i++)
            {
                yield return new Int2(pos.X+i, pos.Y - radius);
            }
            for (int i = -radius; i < radius; i++)
            {
                yield return new Int2(pos.X + radius, pos.Y + i);
            }
            for (int i = -radius; i < radius; i++)
            {
                yield return new Int2(pos.X - i, pos.Y + radius);
            }
            for (int i = -radius; i < radius; i++)
            {
                yield return new Int2(pos.X - radius, pos.Y - i);
            }
            //var hex = this + Directions[0] * radius;
            //for (int i = 0; i < Directions.Length; i++)
            //{
            //    int index = (i + 2) % 6;
            //    var delta = Directions[index];
            //    for (int r = 0; r < radius; r++)
            //    {
            //        yield return hex;
            //        hex += delta;
            //    }
            //}
        }
        //public static int GetCircleHexCount(int radius)
        //{
        //    return radius * 6;
        //}

        //public IEnumerable<Hex> GetCircle(int radius)
        //{
        //    var hex = this + Directions[0] * radius;
        //    for (int i = 0; i < Directions.Length; i++)
        //    {
        //        int index = (i + 2) % 6;
        //        var delta = Directions[index];
        //        for (int r = 0; r < radius; r++)
        //        {
        //            yield return hex;
        //            hex += delta;
        //        }
        //    }
        //}
    }
}
