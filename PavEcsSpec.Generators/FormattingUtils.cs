using System;
using System.Text;

namespace PavEcsSpec.Generators
{
    public static class FormattingUtils 
    {
        public static string PadLeftAllLines(this string data, int width)
        {
            StringBuilder result = new StringBuilder();
            foreach (var line in data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    result.AppendLine(line.PadLeft(width + line.Length));
                }
                else
                {
                    result.AppendLine(line);
                }
            }

            return result.ToString();
        }
    }
}