using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PavEcsGame.Components
{

    public struct SymbolComponent
    {
        public static readonly SymbolComponent Empty = new SymbolComponent() { Value = ' ' };

        public char Value;
        public Depth Depth;

        public ConsoleColor MainColor;

        public SymbolComponent(char value, Depth depth = Depth.Back)
        {
            Value = value;
            Depth = depth;
            MainColor = ConsoleColor.White;
        }

        public override string ToString() =>$"Symbol:{Value}";
    }

    public enum Depth
    {
        Back = 0,
        Foreground = 1
    }
}
