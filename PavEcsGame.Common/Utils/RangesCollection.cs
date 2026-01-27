using System;
using System.Collections.Generic;

namespace PavEcsGame.Utils
{
    public class RangesCollection
    {
        private readonly bool _circular;

        private readonly SortedSet<Range> _data;

        public struct Range
        {
            public float Start;
            public float End;

            public Range(float start, float end)
            {
                Start = start;
                End = end;
            }

            public float Length()
            {
                return End - Start;
            }

            public override string ToString()
            {
                return $"[{Start}-{End}]";
            }
        }

        private class CompareRange : IComparer<Range>
        {
            public int Compare(Range x, Range y)
            {
                var result = 0;
                if (x.End < y.Start)
                    result = -1;
                else if (x.Start > y.End) result = 1;
                return result;
            }
        }

        public RangesCollection(bool circular = false)
        {
            _data = new SortedSet<Range>(new CompareRange());
            _circular = circular;
        }

        public void AddRange(float start, float end)
        {
            AddRange(new Range(start, end));
        }

        private SortedSet<Range> GetViewBetween(Range range)
        {
            return _data.GetViewBetween(new Range(range.Start, range.Start), new Range(range.End, range.End));
        }

        public void AddRange(Range range)
        {
            if (_circular)
            {
                if (range.Start < 0)
                {
                    var subRange = new Range(range.Start + 1, 1);
                    AddRangeInternal(subRange);
                    range.Start = 0;
                }

                if (range.End > 1)
                {
                    var subRange = new Range(0, range.End - 1);
                    AddRangeInternal(subRange);
                    range.End = 1;
                }
            }

            AddRangeInternal(range);
        }

        private void AddRangeInternal(Range range)
        {
            var newRange = range;
            if (_data.Count != 0)
            {
                var ranges = GetViewBetween(range);
                if (ranges.Count != 0)
                {
                    newRange = new Range(
                        Math.Min(range.Start, ranges.Min.Start),
                        Math.Max(range.End, ranges.Max.End)
                    );
                    //remove all of them
                    ranges.RemoveWhere(x => true);
                }
            }

            _data.Add(newRange);
        }


        public float IntersectLength(Range range)
        {
            float result = 0;
            if (_circular)
            {
                if (range.Start < 0)
                {
                    var subRange = new Range(range.Start + 1, 1);
                    result += IntersectLengthInternal(subRange);
                    range.Start = 0;
                }

                if (range.End > 1)
                {
                    var subRange = new Range(0, range.End - 1);
                    result += IntersectLengthInternal(subRange);
                    range.End = 1;
                }
            }

            result += IntersectLengthInternal(range);
            return result;
        }

        private float IntersectLengthInternal(Range range)
        {
            float result = 0;
            var ranges = GetViewBetween(range);
            float start;
            float end;
            foreach (var r in ranges)
            {
                start = Math.Max(range.Start, r.Start);
                end = Math.Min(range.End, r.End);
                result += end - start;
            }

            return result;
        }

        public void Clear()
        {
            _data.Clear();
        }

        public override string ToString()
        {
            return string.Join(",", _data);
        }
    }
}