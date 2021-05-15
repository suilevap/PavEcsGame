using System;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    internal struct RenderItemCommand
    {
        public SymbolComponent Symbol;
        public ConsoleColor BackgroundColor;
        public PositionComponent Position;
    }
}