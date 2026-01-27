using System;
using System.Collections.Generic;
using PavEcsGame.Utils;

namespace PavEcsGame.Area
{
    public abstract class FieldOfViewComputation<T>
        where T : struct
    {
        private readonly RangesCollectionV2 _ranges;

        protected FieldOfViewComputation()
        {
            _ranges = new RangesCollectionV2(true);
        }

        protected abstract IReadOnlyList<T> GetCircle(int radius);

        public void Compute(T startPos, int radius, Func<T, T, bool> hasObstacle, ref (T point, float value)[] result,
            out int count)
        {
            var totalSize = (radius + 1) * 2 * 4 * radius / 2 + 1;
            Helper.EnsureSize(ref result, totalSize);
            _ranges.Clear();
            count = 0;
            result[count++] = (default, 1);
            //yield return (default, 1);

            for (var r = 1; r <= radius; r++)
            {
                var circlePoints = GetCircle(r);

                var cellSize = 1.0f / circlePoints.Count;
                var index = 0;
                for (var i = 0; i < circlePoints.Count; i++)
                {
                    var delta = circlePoints[i];
                    var range = GetRange(index, cellSize);

                    var occluded = _ranges.IntersectLength(range) / cellSize;
                    //yield return (delta, 1f - occluded);
                    result[count++] = (delta, 1f - occluded);

                    if (occluded < 1 && hasObstacle(startPos, delta)) _ranges.AddRange(range);

                    index++;
                }
            }
        }

        private RangesCollectionV2.Range GetRange(int index, float cellsize)
        {
            var result = new RangesCollectionV2.Range(
                (index - 0.5f) * cellsize,
                (index + 0.5f) * cellsize);

            return result;
        }
    }
}