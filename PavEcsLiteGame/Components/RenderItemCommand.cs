using System;

namespace PavEcsGame.Components
{
    public struct RenderItemCommand
    {
        public SymbolComponent Symbol;
        public ConsoleColor BackgroundColor;
        public PositionComponent Position;
    }
}
