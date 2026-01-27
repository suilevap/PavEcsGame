using System;
using PavEcsGame.Components;

namespace PavEcsGame
{
    public class MapData<T> : IMapData<Int2, T>, IMapData<PositionComponent, T>
    {
        private T[,] _data;

        public int Width { get; private set; }
        public int Height { get; private set; }


        public void Init(in Int2 size)
        {
            Width = size.X;
            Height = size.Y;
            _data = new T[Width + 1, Height + 1];
        }

        public Int2 MinPos { get; } = Int2.Zero;
        public Int2 MaxPos => new Int2(Width, Height);


        public ref T GetRef(in Int2 pos)
        {
            return ref _data[pos.X, pos.Y];
        }

        public T Get(in Int2 pos)
        {
            return _data[pos.X, pos.Y];
        }

        public void Set(in Int2 pos, in T item)
        {
            _data[pos.X, pos.Y] = item;
        }

        public Int2 GetSafePos(in Int2 value)
        {
            return new Int2((value.X + Width) % Width, (value.Y + Height) % Height);
        }

        public void Clear()
        {
            if (_data != null) Array.Clear(_data, 0, _data.Length);
        }

        public void Merge<TC, TV2>(IReadOnlyMapData<Int2, TV2> data2, in TC context,
            MergeDelegate<TC, Int2, T, TV2> mergeFunc)
        {
            var pos = new Int2();
            for (pos.Y = MinPos.Y; pos.Y < MaxPos.Y; pos.Y++)
            for (pos.X = MinPos.X; pos.X < MaxPos.X; pos.X++)
                mergeFunc(in context, pos, ref GetRef(pos), data2.Get(pos));
        }


        public void Merge<TC, T2>(IReadOnlyMapData<PositionComponent, T2> data2, in TC context,
            MergeDelegate<TC, PositionComponent, T, T2> mergeFunc)
        {
            var pos = new PositionComponent();
            for (pos.Value.Y = MinPos.Y; pos.Value.Y < MaxPos.Y; pos.Value.Y++)
            for (pos.Value.X = MinPos.X; pos.Value.X < MaxPos.X; pos.Value.X++)
                mergeFunc(in context, pos, ref GetRef(pos), data2.Get(pos));
        }

        public void Fill(in T value)
        {
            var pos = new Int2();
            for (pos.Y = MinPos.Y; pos.Y < MaxPos.Y; pos.Y++)
            for (pos.X = MinPos.X; pos.X < MaxPos.X; pos.X++)
                GetRef(pos) = value;
        }

        public void Fill(in T value1, in T value2)
        {
            var pos = new Int2();
            for (pos.Y = MinPos.Y; pos.Y < MaxPos.Y; pos.Y++)
            for (pos.X = MinPos.X; pos.X < MaxPos.X; pos.X++)
                GetRef(pos) = pos.IsHexPos() ? value1 : value2;
        }

        //public void Merge<T2>(IReadOnlyMapData<PositionComponent, T2> data2, Func<PositionComponent, T, T2, T> mergeFunc)
        //{
        //    PositionComponent pos = new PositionComponent();
        //    for (pos.Value.Y = MinPos.Y; pos.Value.Y < MaxPos.Y; pos.Value.Y++)
        //    {
        //        for (pos.Value.X = MinPos.X; pos.Value.X < MaxPos.X; pos.Value.X++)
        //        {
        //            ref var item = ref GetRef(pos);
        //            item = mergeFunc(pos, item, data2.Get(pos));
        //        }
        //    }
        //}

        //public void Merge<T2>(IReadOnlyMapData<Int2, T2> data2, Func<Int2, T, T2, T> mergeFunc)
        //{
        //    Int2 pos = new Int2();
        //    for (pos.Y = MinPos.Y; pos.Y < MaxPos.Y; pos.Y++)
        //    {
        //        for (pos.X = MinPos.X; pos.X < MaxPos.X; pos.X++)
        //        {
        //            ref var item = ref GetRef(pos);
        //            item = mergeFunc(pos, item, data2.Get(pos));
        //        }
        //    }
        //}

        public void CopyFrom(MapData<T> fromData)
        {
            if (_data != null && fromData != null) Array.Copy(fromData._data, _data, _data.Length);
        }


        #region PositionComponent

        public void Init(in PositionComponent size)
        {
            Init(in size.Value);
        }

        PositionComponent IReadOnlyMapData<PositionComponent, T>.MinPos => new PositionComponent(MinPos);

        PositionComponent IReadOnlyMapData<PositionComponent, T>.MaxPos => new PositionComponent(MaxPos);


        public ref T GetRef(in PositionComponent pos)
        {
            return ref GetRef(in pos.Value);
        }

        public T Get(in PositionComponent pos)
        {
            return Get(in pos.Value);
        }


        public void Set(in PositionComponent pos, in T item)
        {
            Set(in pos.Value, in item);
        }

        public PositionComponent GetSafePos(in PositionComponent value)
        {
            return new PositionComponent(GetSafePos(in value.Value));
        }

        #endregion
    }
}