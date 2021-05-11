using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PavEcsGame.Components
{

    [DebuggerDisplay("S: {Value}")]
    public struct SymbolComponent
    {
        public static readonly SymbolComponent Empty = new SymbolComponent() { Value = ' ' };

        public char Value;

    }
}
