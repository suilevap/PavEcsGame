﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    public interface IReadOnlyMapData<TP, out TV>
    {
        TP MinPos { get; }
        TP MaxPos { get; }

        TV Get(in TP pos);
        TP GetSafePos(in TP pos);

        bool IsValid(in TP pos);
    }

    public interface IMapData<TP, TV> : IReadOnlyMapData<TP, TV>
    {
        void Init(in TP size);

        ref TV GetRef(in TP pos);
        void Set(in TP pos, in TV item);
        void Clear();
        //void Merge<TV2>(IReadOnlyMapData<TP, TV2> data2, Func<TP, TV, TV2, TV> mergeFunc);
        void Merge<TC, TV2>(IReadOnlyMapData<TP, TV2> data2, in TC context, MergeDelegate<TC, TP, TV, TV2> mergeFunc);

    }
    public delegate void MergeDelegate<TC, TP, TV1, TV2>(in TC context, in TP pos, ref TV1 v1, in TV2 v2);


    public static class MapDataExtensions
    {
        //public static IEnumerable<(Int2 pos, TV item)> GetAll<TV>(this IMapData<Int2, TV> data)
        //{
        //    Int2 pos = new Int2();
        //    for (pos.Y = data.MinPos.Y; pos.Y < data.MaxPos.Y; pos.Y++)
        //    {
        //        for (pos.X = data.MinPos.X; pos.X < data.MaxPos.X; pos.X++)
        //        {
        //            var item = data.Get(pos);
        //            //if (item == default(TV))
        //            {
        //                yield return (pos,item);
        //            }
        //        }
        //    }
        //}
   
        public static MapPosEnumerator<TV> GetAll<TV>(
            this IReadOnlyMapData<PositionComponent, TV> data)
        {
            return new MapPosEnumerator<TV>(data);
        }
        public struct MapPosEnumerator<TV> 
        {
            readonly IReadOnlyMapData<PositionComponent, TV> _data;
            readonly int _w;
            readonly int _h;
            Int2 _pos;

            public MapPosEnumerator(IReadOnlyMapData<PositionComponent, TV> data)
            {
                _data = data;
                _pos = new Int2(-1,0);
                _w = data.MaxPos.Value.X;
                _h = data.MaxPos.Value.Y;
            }

            public MapPosEnumerator<TV> GetEnumerator() => this;

            public (PositionComponent, TV) Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => (_pos,_data.Get(_pos));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var x = _pos.X + 1;
                _pos.X = x % _w;
                if (x >= _w)
                {
                    _pos.Y++;
                }
                return _pos.Y < _h;
            }

        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool IsValid<TV>(this IReadOnlyMapData<PositionComponent, TV> data, in PositionComponent pos)
        //{
        //    return (pos.Value.X >= 0 && pos.Value.Y >= 0 && pos.Value.X < data.MaxPos.Value.X && pos.Value.Y < data.MaxPos.Value.Y);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool IsValid<TV>(this IReadOnlyMapData<Int2, TV> data, in Int2 pos)
        //{
        //    return (pos.X >= 0 && pos.Y >= 0 && pos.X < data.MaxPos.X && pos.Y < data.MaxPos.Y);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHexPos(this in Int2 pos)
        {
            return (pos.X + pos.Y % 2) % 2 == 0;
        }

        public static TR CheckNeighbours<TV,TR>(
            this IReadOnlyMapData<PositionComponent, TV> data, 
            in TR initValue, 
            PositionComponent pos, 
            Func<TR, PositionComponent, TV, TR> mergeFunc)
        {
            PositionComponent p = pos;
            TR result = initValue;
            p = pos.Add(0, -1);
            if (data.IsValid(p))
            {
                result = mergeFunc(result, p, data.Get(p));
            }
            
            p = pos.Add(-1, 0);
            if (data.IsValid(p))
            {
                result = mergeFunc(result, p, data.Get(p));
            }

            p = pos.Add(1, 0);
            if (data.IsValid(p))
            {
                result = mergeFunc(result, p, data.Get(p));
            }

            p = pos.Add(0, 1);
            if (data.IsValid(p))
            {
                result = mergeFunc(result, p, data.Get(p));
            }

            return result;
        }

    }
}
