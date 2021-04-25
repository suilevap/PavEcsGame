﻿using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using System;
using System.Collections.Generic;

namespace PavEcsGame.GameLoop
{
    interface IMapData<TP, TV>
    {
        TP MinPos { get; }
        TP MaxPos { get; }

        void Init(in TP size);

        ref TV Get(in TP pos);
        void Set(in TP pos,in TV item);
        TP GetSafePos(in TP value);

    }

    static class MapDataExtensions
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
        public static IEnumerable<(PositionComponent pos, TV item)> GetAll<TV>(this IMapData<PositionComponent, TV> data)
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
