using System;

namespace PavEcsGame.Components
{
    public struct LightValueComponent
    {
        public byte Value;
        public LightType LightType;
    }

    [Flags]
    public enum LightType
    {
        None = 0,
        Fire = 1 << 0,
        Electricity = 1 << 1,
        Acid = 1 << 2,
    }
}
