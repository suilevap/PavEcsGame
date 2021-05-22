using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;

namespace PaveEcsGame.Area
{
    public class FieldOfViewComputationInt2 : FieldOfViewComputation<Int2>
    {
        private static Int2[][] _radiusPoints = new Int2[16][];

        protected override IReadOnlyList<Int2> GetCircle(int radius)
        {
            Int2[] result = null;
            if (radius < _radiusPoints.Length)
            {
                result = _radiusPoints[radius];
            }
            else
            {
                Array.Resize(ref _radiusPoints, radius + 1);
            }

            if (result == null)
            {
                result = GetCirclePoints(Int2.Zero, radius).ToArray();
                _radiusPoints[radius] = result;
            }

            return result;
        }
        //protected override (IEnumerable<Int2> points, int count) GetCircle(Int2 pos, int radius)
        //{
        //    return (GetCirclePoints(pos, radius), GetCircleCount(radius));
        //}

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

        //public struct CircleEnumerator
        //{
        //    private int i;
        //    private int _radius;
        //    private int _part;
        //    public CircleEnumerator(int radius)
        //    {
        //        _radius = radius;
        //    }

        //    public CircleEnumerator GetEnumerator() => this;

        //    public Int2 Current
        //    {
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        get => (_pos, _data.Get(_pos));
        //    }

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    public bool MoveNext()
        //    {
        //        var x = _pos.Value.X + 1;
        //        _pos.Value.X = x % _w;
        //        _pos.Value.Y += x / _w;
        //        return _pos.Value.Y < _h;
        //    }

        //}
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
