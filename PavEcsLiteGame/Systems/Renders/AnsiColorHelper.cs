using System;

namespace PavEcsGame.Systems.Renders
{
    /// <summary>
    /// Utility for converting ConsoleColor to ANSI escape codes.
    /// Uses the pattern: background_code = foreground_code + 10
    /// </summary>
    public static class AnsiColorHelper
    {
        /// <summary>
        /// Get ANSI foreground color code (30-37 or 90-97).
        /// </summary>
        public static int GetForegroundCode(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => 30,
                ConsoleColor.DarkRed => 31,
                ConsoleColor.DarkGreen => 32,
                ConsoleColor.DarkYellow => 33,
                ConsoleColor.DarkBlue => 34,
                ConsoleColor.DarkMagenta => 35,
                ConsoleColor.DarkCyan => 36,
                ConsoleColor.Gray => 37,
                ConsoleColor.DarkGray => 90,
                ConsoleColor.Red => 91,
                ConsoleColor.Green => 92,
                ConsoleColor.Yellow => 93,
                ConsoleColor.Blue => 94,
                ConsoleColor.Magenta => 95,
                ConsoleColor.Cyan => 96,
                ConsoleColor.White => 97,
                _ => 30  // Default to black
            };
        }

        /// <summary>
        /// Get ANSI background color code (40-47 or 100-107).
        /// Background codes are always foreground + 10.
        /// </summary>
        public static int GetBackgroundCode(ConsoleColor color)
        {
            return GetForegroundCode(color) + 10;
        }

        /// <summary>
        /// Get ANSI color code for foreground or background.
        /// </summary>
        public static int GetColorCode(ConsoleColor color, bool isBackground)
        {
            return isBackground ? GetBackgroundCode(color) : GetForegroundCode(color);
        }
    }
}