using System;
using System.Collections.Generic;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    public interface IReadOnlyMapData<TP, out TV>
    {
        TP MinPos { get; }
        TP MaxPos { get; }

        TV Get(in TP pos);
        TP GetSafePos(in TP value);
    }

    public interface IMapData<TP, TV> : IReadOnlyMapData<TP, TV>
    {
        void Init(in TP size);

        ref TV GetRef(in TP pos);
        void Set(in TP pos, in TV item);
        void Clear();
        //void Merge<TV2>(IReadOnlyMapData<TP, TV2> data2, Func<TP, TV, TV2, TV> mergeFunc);
        void Merge<TV2>(IReadOnlyMapData<TP, TV2> data2, MergeDelegate<TP, TV, TV2> mergeFunc);

    }
    public delegate void MergeDelegate<TP, TV1, TV2>(in TP pos, ref TV1 v1, in TV2 v2);


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
        public static IEnumerable<(PositionComponent pos, TV item)> GetAll<TV>(this IReadOnlyMapData<PositionComponent, TV> data)
        {
            PositionComponent pos = new PositionComponent();
            for (pos.Value.Y = data.MinPos.Value.Y; pos.Value.Y < data.MaxPos.Value.Y; pos.Value.Y++)
            {
                for (pos.Value.X = data.MinPos.Value.X; pos.Value.X < data.MaxPos.Value.X; pos.Value.X++)
                {
                    var item = data.Get(pos);
                    //if (item == default(TV))
                    {
                        yield return (pos, item);
                    }
                }
            }
        }

    }
}
