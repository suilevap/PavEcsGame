using System;
using System.Collections.Generic;

namespace PavEcsGame.Utils
{
    public class RangesCollectionV2
    {
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

            public static int Compare(ref Range x, ref Range y)
            {
                int result = 0;
                if (x.End < y.Start)
                {
                    result = -1;
                }
                else if (x.Start > y.End)
                {
                    result = 1;
                }
                return result;
            }
        }

        //class CompareRange : IComparer<Range>
        //{
        //    public int Compare(Range x, Range y) => Range.Compare(x, y);
        //}

        //private SortedSet<Range> _data;
        private readonly bool _circular;
        private readonly LinkedList<Range> _data;

        private Stack<LinkedListNode<Range>> _emptyNodes = new Stack<LinkedListNode<Range>>(15);

        public RangesCollectionV2(bool circular = false)
        {
            // = new SortedSet<Range>(new CompareRange());
            _circular = circular;
            _data = new LinkedList<Range>();
            //for (int i = 0; i < 256; i++)
            //{
            //    _emptyNodes.Push(new LinkedListNode<Range>(default));
            //}
        }
        public void AddRange(float start, float end)
        {
            AddRange(new Range(start, end));
        }

        public void AddRange(Range range)
        {
            if (_circular)
            {
                if (range.Start < 0)
                {
                    Range subRange = new Range(range.Start + 1, 1);
                    AddRangeInternal(subRange);
                    range.Start = 0;
                }

                if (range.End > 1)
                {
                    Range subRange = new Range(0, range.End - 1);
                    AddRangeInternal(subRange);
                    range.End = 1;
                }
            }

            AddRangeInternal(range);
        }

        private void AddRangeInternal(Range range)
        {
            //if (_data.Count != 0)
            //{
            var node = _data.First;//todo: use previous
            var startNode = node;
            LinkedListNode<Range> insertBefore = null;
            while (node != null)
            {
                var currentRange = node.Value;
                var compare = Range.Compare(ref currentRange, ref range);
                if (compare == 0)
                {
                    var toRemove = node;
                    node = node.Next;

                    range = new Range(
                        Math.Min(range.Start, currentRange.Start),
                        Math.Max(range.End, currentRange.End)
                    );
                    _data.Remove(toRemove);
                    _emptyNodes.Push(toRemove);
                    insertBefore = node;
                }
                else
                {
                    if (compare > 0)
                    {
                        if (insertBefore == null)
                        {
                            insertBefore = node;
                        }
                        break;
                    }
                    else
                    {
                        node = node.Next;
                    }
                }
            }

            LinkedListNode<Range> newNode;
            if (_emptyNodes.Count != 0)
            {
                newNode = _emptyNodes.Pop();
                newNode.Value = range;
            }
            else
            {
                newNode = new LinkedListNode<Range>(range);
            }

            if (insertBefore != null)
            {
                _data.AddBefore(insertBefore, newNode);
            }
            else
            {
                _data.AddLast(newNode);
            }
        }


        public float IntersectLength(Range range)
        {
            float result = 0;
            if (_circular)
            {
                if (range.Start < 0)
                {
                    Range subRange = new Range(range.Start + 1, 1);
                    result += IntersectLengthInternal(subRange);
                    range.Start = 0;
                }

                if (range.End > 1)
                {
                    Range subRange = new Range(0, range.End - 1);
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
            var node = _data.First;//todo: use previous

            while (node != null)
            {
                var currentRange = node.Value;
                var compare = Range.Compare(ref currentRange, ref range);
                if (compare == 0)
                {
                    var start = Math.Max(range.Start, currentRange.Start);
                    var end = Math.Min(range.End, currentRange.End);
                    result += end - start;
                }
                else
                {
                    if (compare > 0)
                    {
                        break;
                    }
                }
                node = node.Next;
            }
            //var ranges = GetViewBetween(range);

            //foreach (var r in ranges)
            //{
            //    var start = Math.Max(range.Start, r.Start);
            //    var end = Math.Min(range.End, r.End);
            //    result += end - start;
            //}
            return Math.Min(result, 1);
        }

        public void Clear()
        {
            var node = _data.First;//todo: use previous

            while (node != null)
            {
                _emptyNodes.Push(node);
                node = node.Next;
            }
            _data.Clear();
        }

        public override string ToString()
        {
            return String.Join(",", _data);
        }
    }
}