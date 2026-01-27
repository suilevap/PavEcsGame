using System.Diagnostics;

namespace PavEcsGame.Components
{
    [DebuggerDisplay("Speed: {Speed}")]
    public struct SpeedComponent
    {
        public SpeedComponent(int x, int y)
            : this(new Int2(x, y))
        {
        }

        public SpeedComponent(Int2 speed)
        {
            Speed = speed;
        }

        public Int2 Speed;
    }
}