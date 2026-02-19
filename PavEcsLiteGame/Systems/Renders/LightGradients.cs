using PavEcsGame.Components;

namespace PavEcsGame.Systems.Renders
{
    /// <summary>
    /// Gradient color palettes for smooth 24-bit RGB lighting.
    /// Each gradient is an array of keyframe colors that will be linearly interpolated
    /// using GetByRateLerp() to produce smooth 256-step gradients.
    /// </summary>
    public static class LightGradients
    {
        /// <summary>
        /// Fire gradient: Black → Dark red → Red-orange → Orange → Yellow → White
        /// Warm tones progressing from dim embers to bright flames
        /// </summary>
        public static readonly Color[] FireGradient = new[]
        {
            new Color(0, 0, 0),         // Black
            new Color(64, 0, 0),        // Dark red
            new Color(128, 16, 0),      // Red
            new Color(180, 40, 0),      // Red-orange
            new Color(220, 80, 0),      // Orange
            new Color(255, 150, 0),     // Bright orange
            new Color(255, 200, 50),    // Yellow-orange
            new Color(255, 220, 100),   // Yellow
            new Color(255, 240, 180),   // Pale yellow
            new Color(255, 255, 255)    // White
        };

        /// <summary>
        /// Electricity gradient: Black → Dark blue → Blue → Sky blue → Cyan
        /// Cool electric tones progressing from dim sparks to bright arcs
        /// </summary>
        public static readonly Color[] ElectricityGradient = new[]
        {
            new Color(0, 0, 0),         // Black
            new Color(0, 0, 64),        // Dark blue
            new Color(0, 64, 128),      // Blue
            new Color(0, 128, 200),     // Sky blue
            new Color(50, 150, 255),    // Bright blue
            new Color(100, 200, 255),   // Cyan
            new Color(150, 220, 255),   // Bright cyan
            new Color(200, 230, 255)    // White-cyan
        };

        /// <summary>
        /// Acid gradient: Black → Dark green → Green → Yellow-green → Pale green
        /// Toxic green tones progressing from dim puddles to glowing pools
        /// </summary>
        public static readonly Color[] AcidGradient = new[]
        {
            new Color(0, 0, 0),         // Black
            new Color(0, 64, 0),        // Dark green
            new Color(0, 128, 20),      // Green
            new Color(50, 180, 50),     // Bright green
            new Color(100, 220, 80),    // Yellow-green
            new Color(150, 255, 100),   // Pale green
            new Color(200, 255, 200)    // Very pale green
        };

        /// <summary>
        /// None/ambient gradient: Dark gray → Gray → Light gray → White
        /// Neutral grayscale for non-typed ambient lighting.
        /// Starts with DarkGray (not black) to maintain visibility in visited areas with low/zero light.
        /// </summary>
        public static readonly Color[] NoneGradient = new[]
        {
            new Color(64, 64, 64),      // Dark gray (minimum visibility for visited areas)
            new Color(96, 96, 96),      // Medium-dark gray
            new Color(128, 128, 128),   // Gray
            new Color(160, 160, 160),   // Medium-light gray
            new Color(192, 192, 192),   // Light gray
            new Color(220, 220, 220),   // Very light gray
            new Color(255, 255, 255)    // White
        };
    }
}
