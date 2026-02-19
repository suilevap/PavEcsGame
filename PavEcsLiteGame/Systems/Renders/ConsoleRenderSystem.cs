using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    /// <summary>
    /// Legacy 16-color console renderer. Not actively used (UseOptimizedRenderSystem=true uses ConsoleAnsiRenderSystem).
    /// Kept for backwards compatibility but does not support 24-bit RGB colors.
    /// </summary>
    public partial class ConsoleRenderSystem : IEcsInitSystem, IEcsRunSystem, IEcsSystemSpec
    {
        private readonly List<EcsUnsafeEntity>[] _groupedbyColor = new List<EcsUnsafeEntity>[16];

        /// <summary>
        /// Convert 24-bit RGB color to 16-color ConsoleColor (lossy conversion for legacy renderer).
        /// </summary>
        private static ConsoleColor ToConsoleColor(Color c)
        {
            var index = (c.R > 128 || c.G > 128 || c.B > 128) ? 8 : 0; // Bright bit
            index |= c.R > 64 ? 4 : 0; // Red bit
            index |= c.G > 64 ? 2 : 0; // Green bit
            index |= c.B > 64 ? 1 : 0; // Blue bit
            return (ConsoleColor)index;
        }

        [Entity]
        private partial struct RenderCommandEntity
        {
            public partial RequiredComponent<RenderItemCommand> Command();
        }

        public void Init(IEcsSystems systems)
        {
            for (var i = 0; i < _groupedbyColor.Length; i++) _groupedbyColor[i] = new List<EcsUnsafeEntity>(128);
            Console.CursorVisible = false;
        }

        public void Run(IEcsSystems systems)
        {
            RenderGroupedByMainColor();
            //RenderSimple();
        }

        private void RenderGroupedByMainColor()
        {
            foreach (var ent in _providers.RenderCommandEntityProvider)
            {
                var key = (int)ToConsoleColor(ent.Command().Get().Symbol.MainColor);
                _groupedbyColor[key].Add((EcsUnsafeEntity)ent.GetRawId());
            }

            var commandPool = _providers.RenderCommandEntityProvider._commandPool;
            for (var i = 0; i < _groupedbyColor.Length; i++)
            {
                var list = _groupedbyColor[i];
                if (list.Count != 0)
                {
                    var mainColor = (ConsoleColor)i;
                    Console.ForegroundColor = mainColor;

                    foreach (var entId in list)
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
                    Console.SetCursorPosition(item.Position.Value.X, item.Position.Value.Y);
                var bgColor = ToConsoleColor(item.BackgroundColor);
                if (Console.BackgroundColor != bgColor) Console.BackgroundColor = bgColor;
                if (item.Symbol.Value != default)
                    Console.Write(item.Symbol.Value);
                else
                    //Console.ResetColor();
                    Console.Write(SymbolComponent.Empty.Value);
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