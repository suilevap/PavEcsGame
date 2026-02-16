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
        public ConsoleColor[] FgColors;
        public ConsoleColor[] BgColors;

        public static TextPanelComponent Create(int row, int col, int width, int lineCount)
        {
            var panel = new TextPanelComponent
            {
                Row = row,
                Col = col,
                Width = width,
                LineCount = lineCount,
                Lines = new string[lineCount],
                FgColors = new ConsoleColor[lineCount],
                BgColors = new ConsoleColor[lineCount]
            };
            for (int i = 0; i < lineCount; i++)
            {
                panel.Lines[i] = string.Empty;
                panel.FgColors[i] = ConsoleColor.Gray;
                panel.BgColors[i] = ConsoleColor.Black;
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
            panel.FgColors = new ConsoleColor[lineCount];
            panel.BgColors = new ConsoleColor[lineCount];
            for (int i = 0; i < lineCount; i++)
            {
                panel.Lines[i] = string.Empty;
                panel.FgColors[i] = ConsoleColor.Gray;
                panel.BgColors[i] = ConsoleColor.Black;
            }
        }

        public static void SetLine(ref TextPanelComponent panel, int line, string text,
            ConsoleColor fg = ConsoleColor.Gray, ConsoleColor bg = ConsoleColor.Black)
        {
            if (line < 0 || line >= panel.LineCount)
                return;
            panel.Lines[line] = text;
            panel.FgColors[line] = fg;
            panel.BgColors[line] = bg;
        }

        public static void Clear(ref TextPanelComponent panel)
        {
            for (int i = 0; i < panel.LineCount; i++)
            {
                panel.Lines[i] = string.Empty;
                panel.FgColors[i] = ConsoleColor.Gray;
                panel.BgColors[i] = ConsoleColor.Black;
            }
        }
    }
}
