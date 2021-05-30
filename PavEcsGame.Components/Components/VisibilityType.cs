using System;

namespace PavEcsGame.Components
{
    [Flags]
    public enum VisibilityType
    {
        None = 0,
        Visible = 1 << 0,
        Known = 1 << 1,
    }

}
