using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsGame;
using PavEcsGame.Utils;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    public partial class ConsoleRenderSystem : IEcsInitSystem, IEcsRunSystem, IEcsSystemSpec
    {

        [Entity]
        private partial struct RenderCommandEntity
        {
            public partial RequiredComponent<RenderItemCommand> Command();
        }


        private List<EcsUnsafeEntity>[] _groupedbyColor = new List<EcsUnsafeEntity>[16];

        public void Init(EcsSystems systems)
        {
            for (int i = 0; i < _groupedbyColor.Length; i++)
            {
                _groupedbyColor[i] = new List<EcsUnsafeEntity>(128);
            }
            Console.CursorVisible = false;
        }
        public void Run(EcsSystems systems)
        {
            RenderGroupedByMainColor();
            //RenderSimple();
        }
        private void RenderGroupedByMainColor()
        {

            foreach(var ent in _providers.RenderCommandEntityProvider)
            {
                var key = (int)ent.Command().Get().Symbol.MainColor;
                _groupedbyColor[key].Add((EcsUnsafeEntity) ent.GetRawId());
            }

            var commandPool = _providers.RenderCommandEntityProvider._commandPool;
            for (int i = 0; i < _groupedbyColor.Length; i++)
            {
                var list = _groupedbyColor[i];
                if (list.Count != 0)
                {
                    ConsoleColor mainColor = (ConsoleColor)i;
                    Console.ForegroundColor = mainColor;

                    foreach (EcsUnsafeEntity entId in list)
                    {
                        RenderItemWithoutColor(in commandPool.Get(entId));
                        commandPool.Del(entId);
                    }
                    list.Clear();
                }
            }

            static void RenderItemWithoutColor(in RenderItemCommand item)
            {
                if (Console.CursorLeft != item.Position.Value.X || Console.CursorTop != item.Position.Value.Y)
                {
                    Console.SetCursorPosition(item.Position.Value.X, item.Position.Value.Y);
                }
                if (Console.BackgroundColor != item.BackgroundColor)
                {
                    Console.BackgroundColor = item.BackgroundColor;
                }
                if (item.Symbol.Value != default)
                {
                    Console.Write(item.Symbol.Value);
                }
                else
                {
                    //Console.ResetColor();
                    Console.Write(SymbolComponent.Empty.Value);
                }
            }
        }

        //private void RenderSimple()
        //{

        //    var commandPool = _renderCommandSpec.Include.Pool1;
        //    foreach (EcsUnsafeEntity ent in _renderCommandSpec.Filter)
        //    {
        //        RenderItem(in commandPool.Get(ent));
        //        commandPool.Del(ent);
        //    }

        //    static void RenderItem(in RenderItemCommand item)
        //    {
        //        Console.SetCursorPosition(item.Position.Value.X, item.Position.Value.Y);
        //        if (item.Symbol.Value != default)
        //        {
        //            Console.ForegroundColor = item.Symbol.MainColor;
        //            Console.BackgroundColor = item.BackgroundColor;
        //            Console.Write(item.Symbol.Value);
        //        }
        //        else
        //        {
        //            Console.ResetColor();
        //            Console.Write(SymbolComponent.Empty.Value);
        //        }
        //    }

        //}
    }
}
