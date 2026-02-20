using System;

namespace PavEcsGame.Components
{
    public struct LightValueComponent
    {
        /// <summary>
        /// R,G,B = accumulated light color (additive blending).
        /// A = bitmask of LightType flags that have participated.
        /// </summary>
        public Color AccumulatedColor;

        /// <summary>Which light types have contributed to this tile (OR of all sources).</summary>
        public LightType LightTypes => (LightType)AccumulatedColor.A;

        /// <summary>Opaque RGB color for rendering — alpha stripped (set to 255).</summary>
        public Color RgbColor => new Color(AccumulatedColor, 255);
    }

    [Flags]
    public enum LightType
    {
        None = 0,
        Fire = 1 << 0,
        Electricity = 1 << 1,
        Acid = 1 << 2
    }
}