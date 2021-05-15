using System;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;

namespace PaveEcsGame
{
    public class MapData<T> : IMapData<Int2, T>, IMapData<PositionComponent, T>
    {

        private T[,] _data;

        public MapData() { }


        public void Init(in Int2 size)
        {
            Width = size.X;
            Height = size.Y;
            _data = new T[Width + 1, Height + 1];
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Int2 MinPos { get; } = Int2.Zero;
        public Int2 MaxPos => new Int2(Width, Height);


        public ref T GetRef(in Int2 pos) => ref _data[pos.X, pos.Y];
        public T Get(in Int2 pos) => _data[pos.X, pos.Y];

        public void Set(in Int2 pos, in T item) => _data[pos.X, pos.Y] = item;

        public Int2 GetSafePos(in Int2 value) => new Int2((value.X + Width) % Width, (value.Y + Height) % Height);


        #region PositionComponent

        public void Init(in PositionComponent size) => Init(in size.Value);

        PositionComponent IReadOnlyMapData<PositionComponent, T>.MinPos => new PositionComponent(MinPos);

        PositionComponent IReadOnlyMapData<PositionComponent, T>.MaxPos => new PositionComponent(MaxPos);


        public ref T GetRef(in PositionComponent pos) => ref GetRef(in pos.Value);

        public T Get(in PositionComponent pos) => Get(in pos.Value);


        public void Set(in PositionComponent pos, in T item) => Set(in pos.Value, in item);
        public PositionComponent GetSafePos(in PositionComponent value) => new PositionComponent(GetSafePos(in value.Value));
        #endregion

        public void Clear()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        public void Merge(IReadOnlyMapData<PositionComponent, T> data2, Func<PositionComponent, T, T, T> mergeFunc)
        {
            PositionComponent pos = new PositionComponent();
            for (pos.Value.Y = MinPos.Y; pos.Value.Y < MaxPos.Y; pos.Value.Y++)
            {
                for (pos.Value.X = MinPos.X; pos.Value.X < MaxPos.X; pos.Value.X++)
                {
                    ref var item = ref GetRef(pos);
                    item = mergeFunc(pos, item, data2.Get(pos));
                }
            }
        }

        public void Merge(IReadOnlyMapData<Int2, T> data2, Func<Int2, T, T, T> mergeFunc)
        {
            Int2 pos = new Int2();
            for (pos.Y = MinPos.Y; pos.Y < MaxPos.Y; pos.Y++)
            {
                for (pos.X = MinPos.X; pos.X < MaxPos.X; pos.X++)
                {
                    ref var item = ref GetRef(pos);
                    item = mergeFunc(pos, item, data2.Get(pos));
                }
            }
        }
    }

}
