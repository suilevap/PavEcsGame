using System;

namespace PavEcsGame.Components
{
    internal struct RenderItemCommand
    {
        public SymbolComponent Symbol;
        public Color BackgroundColor;
        public PositionComponent Position;
    }
}