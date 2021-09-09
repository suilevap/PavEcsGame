using PavEcsGame.Components;
using DrawingColor = System.Drawing.Color;

namespace PavEcsGame
{

    public static class ColorExtensions

    {
        //https://stackoverflow.com/questions/1988833/converting-color-to-consolecolor/12340136
        public static System.ConsoleColor ToConsoleColor(this in Color c)
        {
            int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0; // Bright bit
            index |= (c.R > 64) ? 4 : 0; // Red bit
            index |= (c.G > 64) ? 2 : 0; // Green bit
            index |= (c.B > 64) ? 1 : 0; // Blue bit
            return (System.ConsoleColor)index;
        }

        public static DrawingColor ToDrawingColor(this in Color c)
        {
            return DrawingColor.FromArgb(c.R, c.G, c.B);
        }

        public static Color ToColor(this in DrawingColor color)
        {
            return new Color(color.R, color.G, color.B);
        }

    }
}
