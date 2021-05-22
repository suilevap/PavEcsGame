using System;
using System.Collections.Generic;
using System.Data.Common;

namespace PaveEcsGame.Area
{
    public abstract class FieldOfViewComputation<T>
        where T: struct
    {

        private readonly Utils.RangesCollectionV2 _ranges;

        protected FieldOfViewComputation()
        {
            _ranges = new Utils.RangesCollectionV2(true);
        }

        protected abstract IReadOnlyList<T> GetCircle(int radius);

        public void Compute(T startPos, int radius, Func<T, T, bool> hasObstacle, ref (T point, float value)[] result, out int count)
        {
            var totalSize = (radius + 1) * 2 * 4 * radius / 2 + 1;
            Utils.Helper.EnsureSize(ref result, totalSize);
            _ranges.Clear();
            count  = 0;
            result[count++] = (default, 1);
            //yield return (default, 1);

            for (int r = 1; r <= radius; r++)
            {
                var circlePoints = GetCircle( r);

                float cellSize = 1.0f / circlePoints.Count;
                int index = 0;
                for (var i = 0; i < circlePoints.Count; i++)
                {
                    var delta = circlePoints[i];
                    var range = GetRange(index, cellSize);

                    float occluded = _ranges.IntersectLength(range) / cellSize;
                    //yield return (delta, 1f - occluded);
                    result[count++] = (delta, 1f - occluded);

                    if (occluded < 1 && hasObstacle(startPos, delta))
                    {
                        _ranges.AddRange(range);
                    }

                    index++;
                }
            }

        }

        private Utils.RangesCollectionV2.Range GetRange(int index, float cellsize)
        {
            var result = new Utils.RangesCollectionV2.Range(
                ((float)index - 0.5f) * cellsize,
                ((float)index + 0.5f) * cellsize);

            return result;
        }
    }
}