using System;
using System.Collections.Generic;

namespace PaveEcsGame.Area
{
    public abstract class FieldOfViewComputation<T>
        where T: struct
    {

        private readonly Utils.RangesCollection _ranges;

        protected FieldOfViewComputation()
        {
            _ranges = new Utils.RangesCollection(true);
        }

        protected abstract (IEnumerable<T> points, int count) GetCircle(T pos, int radius);

        public IEnumerable<(T point, float)> Compute(T startPos, int radius, Func<T, bool> hasObstacle)
        {
            _ranges.Clear();
            yield return (startPos, 1);

            for (int r = 1; r <= radius; r++)
            {
                var (circlePoints, count) = GetCircle(startPos, r);

                float cellSize = 1.0f / count;
                int index = 0;
                foreach (var point in circlePoints)
                {
                    var range = GetRange(index, cellSize);

                    float occluded = _ranges.IntersectLength(range) / cellSize;
                    yield return (point, 1f - occluded);
                    if (occluded < 1 && hasObstacle(point))
                    {
                        _ranges.AddRange(range);
                    }
                    index++;
                }
            }
        }

        private Utils.RangesCollection.Range GetRange(int index, float cellsize)
        {
            Utils.RangesCollection.Range result = new Utils.RangesCollection.Range(
                ((float)index - 0.5f) * cellsize,
                ((float)index + 0.5f) * cellsize);

            return result;
        }
    }
}