using System;

namespace PavEcsGame.Components
{
    internal struct TextPanelComponent
    {
        public int Row;
        public int Col;
        public int Width;
        public int LineCount;
        public string[] Lines;
        public Color[] FgColors;
        public Color[] BgColors;

        public static TextPanelComponent Create(int row, int col, int width, int lineCount)
        {
            var panel = new TextPanelComponent
            {
                Row = row,
                Col = col,
                Width = width,
                LineCount = lineCount,
                Lines = new string[lineCount],
                FgColors = new Color[lineCount],
                BgColors = new Color[lineCount]
            };
            for (int i = 0; i < lineCount; i++)
            {
                panel.Lines[i] = string.Empty;
                panel.FgColors[i] = new Color(128, 128, 128); // Gray
                panel.BgColors[i] = Color.Zero; // Black
            }
            return panel;
        }

        public static void Resize(ref TextPanelComponent panel, int row, int col, int width, int lineCount)
        {
            if (panel.Row == row && panel.Col == col && panel.Width == width && panel.LineCount == lineCount)
                return;

            panel.Row = row;
            panel.Col = col;
            panel.Width = width;
            panel.LineCount = lineCount;
            panel.Lines = new string[lineCount];
            panel.FgColors = new Color[lineCount];
            panel.BgColors = new Color[lineCount];
            for (int i = 0; i < lineCount; i++)
            {
                panel.Lines[i] = string.Empty;
                panel.FgColors[i] = new Color(128, 128, 128); // Gray
                panel.BgColors[i] = Color.Zero; // Black
            }
        }

        public static void SetLine(ref TextPanelComponent panel, int line, string text,
            Color? fg = null, Color? bg = null)
        {
            if (line < 0 || line >= panel.LineCount)
                return;
            panel.Lines[line] = text;
            panel.FgColors[line] = fg ?? new Color(128, 128, 128); // Gray
            panel.BgColors[line] = bg ?? Color.Zero; // Black
        }

        public static void Clear(ref TextPanelComponent panel)
        {
            for (int i = 0; i < panel.LineCount; i++)
            {
                panel.Lines[i] = string.Empty;
                panel.FgColors[i] = new Color(128, 128, 128); // Gray
                panel.BgColors[i] = Color.Zero; // Black
            }
        }
    }
}
