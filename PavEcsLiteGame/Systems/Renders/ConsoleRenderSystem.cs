using System;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PaveEcsGame;
using PaveEcsGame.Utils;

namespace PavEcsGame.Systems.Renders
{
    public class ConsoleRenderSystem : IEcsInitSystem, IEcsRunSystem
    {

        private readonly EcsFilterSpec<EcsSpec<RenderItemCommand>, EcsSpec, EcsSpec> _renderCommandSpec;

        public ConsoleRenderSystem(EcsUniverse universe)
        {
            universe
                .Build(ref _renderCommandSpec);
        }

        public void Init(EcsSystems systems)
        {
            Console.CursorVisible = false;
        }

        public void Run(EcsSystems systems)
        {

            var commandPool = _renderCommandSpec.Include.Pool1;
            foreach (var ent in _renderCommandSpec.Filter)
            {
                RenderItem(in commandPool.Get(ent));
                commandPool.Del(ent);
            }

            static void RenderItem(in RenderItemCommand item)
            {
                Console.SetCursorPosition(item.Position.Value.X, item.Position.Value.Y);
                if (item.Symbol.Value != default)
                {
                    Console.ForegroundColor = item.Symbol.MainColor;
                    Console.BackgroundColor = item.BackgroundColor;
                    Console.Write(item.Symbol.Value);
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(SymbolComponent.Empty.Value);
                }
            }

        }
    }
}
