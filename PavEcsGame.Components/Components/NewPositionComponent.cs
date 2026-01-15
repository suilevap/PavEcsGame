using System.Diagnostics;

namespace PavEcsGame.Components
{
    [DebuggerDisplay("NextPos: {Value}")]
    public struct NewPositionComponent
    {
        public PositionComponent? Value;
    }
}
