using System;

namespace PavEcsGame.Components
{
    public struct SymbolComponent
    {
        public static readonly SymbolComponent Empty = new() { Value = ' ' };

        public char Value;
        public Depth Depth;

        public ConsoleColor MainColor;

        public SymbolComponent(char value, Depth depth = Depth.Back)
        {
            Value = value;
            Depth = depth;
            MainColor = ConsoleColor.White;
        }

        public bool IsEmpty => Value == Empty.Value || Value == default;

        public override string ToString()
        {
            return $"Symbol:{Value}";
        }
    }

    public enum Depth : byte
    {
        Back = 0,
        Foreground = 1
    }
}
