using System.IO;
using PavEcsGame.Common.Utils;

namespace PavEcsGame.Tiles
{
    public class TileRule
    {
        public static TileRule Wall = new TileRule(
            new byte[] { (byte)'X', 210, 198, 201, 208, 186, 200, 204, 181, 187, 205, 203, 188, 185, 202, 206 });

        public readonly char[] Symbols;

        public TileRule(byte[] symbols)
        {
            //Symbols = symbols;
            Symbols = new char[symbols.Length];
            for (var i = 0; i < symbols.Length; i++) Symbols[i] = (char)symbols[i];
        }

        public TileRule(char[] symbols)
        {
            Symbols = symbols;
        }

        public override string ToString()
        {
            return string.Join(",", Symbols);
        }

        public static TileRule Load(string name)
        {
            var filename = FileHelper.ResolvePath($"Data/{name}.txt");
            if (!File.Exists(filename))
                return null;
            var lines = File.ReadAllLines(filename);

            var mask = new int[lines[0].Length, lines.Length];
            var result = new char[16];

            for (var y = 0; y < lines.Length - 1; y++)
            for (var x = 0; x < lines[y].Length - 1; x++)
            {
                if (lines[y][x] == '.')
                    continue;
                ;
                if (lines[y][x + 1] != '.')
                {
                    mask[x, y] |= 1 << 0;
                    mask[x + 1, y] |= 1 << 2;
                }

                if (lines[y + 1][x] != '.')
                {
                    mask[x, y] |= 1 << 1;
                    mask[x, y + 1] |= 1 << 3;
                }

                var m = mask[x, y];
                result[m] = lines[y][x];
            }

            return new TileRule(result);
        }
    }
}