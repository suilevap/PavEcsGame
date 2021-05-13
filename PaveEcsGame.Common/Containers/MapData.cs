using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;

namespace PavEcsGame
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
    }

}
