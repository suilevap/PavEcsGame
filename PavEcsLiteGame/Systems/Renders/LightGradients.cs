using PavEcsGame.Components;
using PavEcsGame.Utils;

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

        // ============================================================================
        // Braille Pattern Gradients for Sub-Pixel Density Rendering
        // ============================================================================
        // These gradients use symmetric dot patterns to create visually balanced
        // light intensity representations. Patterns are designed to be vertically
        // and horizontally symmetric for aesthetic consistency.
        //
        // Dot layout reference (2×4 grid):
        //   │ 0 │ 3 │  Row 0 (top)
        //   │ 1 │ 4 │  Row 1
        //   │ 2 │ 5 │  Row 2
        //   │ 6 │ 7 │  Row 3 (bottom)
        // ============================================================================

        /// <summary>
        /// Braille patterns for Fire light intensity.
        /// Center-out radial growth with dithered expansion (symmetric).
        /// </summary>
        public static readonly BrailleChar[] FireBrailleGradient = new[]
        {
            new BrailleChar(0b00000000), // ⠀ empty (dark)
            new BrailleChar(0b00010000), // ⠐ center dot (dim ember)
            new BrailleChar(0b00010010), // ⠒ center row horizontal (symmetric)
            new BrailleChar(0b00011011), // ⠚ center + top row (expanding up)
            new BrailleChar(0b00110110), // ⠶ center two rows (symmetric vertical)
            new BrailleChar(0b00111111), // ⠿ top 3 rows (growing)
            new BrailleChar(0b01111111), // ⡿ add bottom-left (asymmetric dither)
            new BrailleChar(0b11111110), // ⣾ all but bottom-right (dithered)
            new BrailleChar(0b11111111), // ⣿ full (bright flames)
            BrailleChar.Full             // ⣿ max brightness
        };

        /// <summary>
        /// Braille patterns for Electricity light intensity.
        /// Diagonal/cross pattern expanding from center (symmetric dithering).
        /// </summary>
        public static readonly BrailleChar[] ElectricityBrailleGradient = new[]
        {
            new BrailleChar(0b00000000), // ⠀ empty (dark)
            new BrailleChar(0b00010010), // ⠒ center row (symmetric spark)
            new BrailleChar(0b01010010), // ⡒ center + bottom-left (diagonal start)
            new BrailleChar(0b01011011), // ⡛ expanding cross pattern
            new BrailleChar(0b11011011), // ⣛ wider cross (dithered symmetric)
            new BrailleChar(0b11111111), // ⣿ full arc
            new BrailleChar(0b11111111), // ⣿ sustained
            BrailleChar.Full             // ⣿ max brightness
        };

        /// <summary>
        /// Braille patterns for Acid light intensity.
        /// Bottom-center growth with symmetric expansion (puddle spreading).
        /// </summary>
        public static readonly BrailleChar[] AcidBrailleGradient = new[]
        {
            new BrailleChar(0b00000000), // ⠀ empty (dark)
            new BrailleChar(0b01000000), // ⡀ bottom-left center (tiny puddle)
            new BrailleChar(0b11000000), // ⣀ bottom row (symmetric puddle)
            new BrailleChar(0b11010010), // ⣒ bottom + middle row (expanding up)
            new BrailleChar(0b11110110), // ⣶ bottom 3 rows (symmetric)
            new BrailleChar(0b11111111), // ⣿ full (glowing pool)
            BrailleChar.Full             // ⣿ max brightness
        };

        /// <summary>
        /// Braille patterns for None/ambient light intensity.
        /// Center-out radial expansion with balanced dithering (neutral).
        /// </summary>
        public static readonly BrailleChar[] NoneBrailleGradient = new[]
        {
            new BrailleChar(0b00000000), // ⠀ empty (dark)
            new BrailleChar(0b00010010), // ⠒ center row (symmetric)
            new BrailleChar(0b00110110), // ⠶ center two rows (symmetric vertical)
            new BrailleChar(0b00111111), // ⠿ top 3 rows (expanding)
            new BrailleChar(0b11111110), // ⣾ nearly full (dithered)
            new BrailleChar(0b11111111), // ⣿ full (bright)
            BrailleChar.Full             // ⣿ max brightness
        };

        /// <summary>
        /// Get Braille pattern for a given light type and intensity.
        /// Uses linear interpolation between gradient keyframes for smooth transitions.
        /// </summary>
        /// <param name="lightType">Type of light (Fire, Electricity, Acid, or None)</param>
        /// <param name="intensity">Light intensity [0-255]</param>
        /// <returns>Braille pattern representing the light's visual density</returns>
        public static BrailleChar GetBraillePattern(LightType lightType, byte intensity)
        {
            BrailleChar[] gradient = lightType switch
            {
                LightType.Fire => FireBrailleGradient,
                LightType.Electricity => ElectricityBrailleGradient,
                LightType.Acid => AcidBrailleGradient,
                _ => NoneBrailleGradient
            };

            return gradient.GetByRateLerp(intensity);
        }
    }
}
