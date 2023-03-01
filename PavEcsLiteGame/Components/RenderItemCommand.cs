using System;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    public struct RenderItemCommand
    {
        public SymbolComponent Symbol;
        public ConsoleColor BackgroundColor;
        public PositionComponent Position;
    }
}