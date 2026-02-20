using System;

namespace PavEcsGame.Components
{
    /// <summary>
    /// Utility for working with Braille Unicode characters (U+2800–U+28FF).
    /// Each Braille character is a 2×4 grid of dots, allowing sub-pixel rendering in the console.
    ///
    /// Dot positions:
    ///   Column 0  Column 1
    ///   ┌─────┬─────┐
    ///   │  0  │  3  │  Row 0 (top)
    ///   │  1  │  4  │  Row 1
    ///   │  2  │  5  │  Row 2
    ///   │  6  │  7  │  Row 3 (bottom)
    ///   └─────┴─────┘
    ///
    /// Unicode offset from U+2800 base:
    /// - Bit 0 → dot 0 (top-left)
    /// - Bit 1 → dot 1
    /// - Bit 2 → dot 2
    /// - Bit 3 → dot 3 (top-right)
    /// - Bit 4 → dot 4
    /// - Bit 5 → dot 5
    /// - Bit 6 → dot 6 (bottom-left)
    /// - Bit 7 → dot 7 (bottom-right)
    /// </summary>
    public readonly struct BrailleChar : IEquatable<BrailleChar>
    {
        private const int BrailleBase = 0x2800;

        /// <summary>
        /// Bitmask for the 8 dots (0-7). Each bit represents one dot.
        /// </summary>
        public readonly byte Dots;

        /// <summary>
        /// Empty Braille character (no dots).
        /// </summary>
        public static readonly BrailleChar Empty = new BrailleChar(0);

        /// <summary>
        /// Full Braille character (all 8 dots on).
        /// </summary>
        public static readonly BrailleChar Full = new BrailleChar(0xFF);

        public BrailleChar(byte dots)
        {
            Dots = dots;
        }

        /// <summary>
        /// Create Braille character from individual dot states.
        /// </summary>
        public BrailleChar(bool d0, bool d1, bool d2, bool d3, bool d4, bool d5, bool d6, bool d7)
        {
            Dots = (byte)(
                (d0 ? 1 : 0) |
                (d1 ? 2 : 0) |
                (d2 ? 4 : 0) |
                (d3 ? 8 : 0) |
                (d4 ? 16 : 0) |
                (d5 ? 32 : 0) |
                (d6 ? 64 : 0) |
                (d7 ? 128 : 0)
            );
        }

        /// <summary>
        /// Create Braille character from 2×4 boolean grid (column-major).
        /// </summary>
        public static BrailleChar FromGrid(bool[,] grid)
        {
            if (grid.GetLength(0) != 2 || grid.GetLength(1) != 4)
                throw new ArgumentException("Grid must be 2×4 (cols×rows)");

            return new BrailleChar(
                grid[0, 0], grid[0, 1], grid[0, 2],
                grid[1, 0], grid[1, 1], grid[1, 2],
                grid[0, 3], grid[1, 3]
            );
        }

        /// <summary>
        /// Get the Unicode character for this Braille pattern.
        /// </summary>
        public char ToChar() => (char)(BrailleBase + Dots);

        /// <summary>
        /// Check if a specific dot is set (0-7).
        /// </summary>
        public bool IsDotSet(int dotIndex)
        {
            if (dotIndex < 0 || dotIndex > 7)
                throw new ArgumentOutOfRangeException(nameof(dotIndex), "Dot index must be 0-7");
            return (Dots & (1 << dotIndex)) != 0;
        }

        /// <summary>
        /// Set a specific dot (0-7) on or off.
        /// </summary>
        public BrailleChar WithDot(int dotIndex, bool value)
        {
            if (dotIndex < 0 || dotIndex > 7)
                throw new ArgumentOutOfRangeException(nameof(dotIndex), "Dot index must be 0-7");

            byte newDots = value
                ? (byte)(Dots | (1 << dotIndex))
                : (byte)(Dots & ~(1 << dotIndex));

            return new BrailleChar(newDots);
        }

        /// <summary>
        /// Combine two Braille patterns using OR (union of dots).
        /// </summary>
        public static BrailleChar operator |(BrailleChar a, BrailleChar b)
        {
            return new BrailleChar((byte)(a.Dots | b.Dots));
        }

        /// <summary>
        /// Combine two Braille patterns using AND (intersection of dots).
        /// </summary>
        public static BrailleChar operator &(BrailleChar a, BrailleChar b)
        {
            return new BrailleChar((byte)(a.Dots & b.Dots));
        }

        /// <summary>
        /// Invert all dots (XOR with 0xFF).
        /// </summary>
        public static BrailleChar operator ~(BrailleChar a)
        {
            return new BrailleChar((byte)(a.Dots ^ 0xFF));
        }

        /// <summary>
        /// Get density (0.0 - 1.0) based on how many dots are set.
        /// </summary>
        public float Density
        {
            get
            {
                int count = 0;
                for (int i = 0; i < 8; i++)
                    if ((Dots & (1 << i)) != 0) count++;
                return count / 8f;
            }
        }

        /// <summary>
        /// Create Braille pattern from density value (0.0 - 1.0).
        /// Uses dithering pattern for intermediate densities.
        /// </summary>
        public static BrailleChar FromDensity(float density)
        {
            density = Math.Max(0f, Math.Min(1f, density));
            int dotCount = (int)(density * 8 + 0.5f);

            // Ordered dithering pattern for smooth gradients
            // Pattern fills dots in a visually balanced order
            byte[] fillOrder = { 0, 3, 1, 4, 2, 5, 6, 7 };

            byte dots = 0;
            for (int i = 0; i < dotCount; i++)
            {
                dots |= (byte)(1 << fillOrder[i]);
            }

            return new BrailleChar(dots);
        }

        public override string ToString() => $"Braille('{ToChar()}', dots={Convert.ToString(Dots, 2).PadLeft(8, '0')})";

        public bool Equals(BrailleChar other) => Dots == other.Dots;
        public override bool Equals(object obj) => obj is BrailleChar other && Equals(other);
        public override int GetHashCode() => Dots.GetHashCode();
        public static bool operator ==(BrailleChar a, BrailleChar b) => a.Dots == b.Dots;
        public static bool operator !=(BrailleChar a, BrailleChar b) => a.Dots != b.Dots;
    }
}