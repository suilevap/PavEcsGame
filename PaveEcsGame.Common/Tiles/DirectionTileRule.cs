using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using System.Diagnostics;
using System.IO;

namespace PaveEcsGame.Tiles
{
    public class DirectionTileRule
    {
        public readonly char[] Symbols;

        public char GetSymbol(Direction dir) => Symbols[(int)dir];

        public DirectionTileRule(byte[] symbols)
        {
            //Symbols = symbols;
            Symbols = new char[symbols.Length];
            for (int i = 0; i < symbols.Length; i++)
            {
                Symbols[i] = (char) symbols[i];
            }
        }
        public DirectionTileRule(char[] symbols)
        {
            Symbols = symbols;
        }

        public override string ToString()
        {
            return string.Join(",", Symbols);
        }

        public static DirectionTileRule Load(string name)
        {
            var filename = $"Data/{name}.txt";
            if (!File.Exists(filename))
                return null;
            var lines = File.ReadAllLines(filename);

            char[] result = new char[5];
            var h = lines.Length;
            var w = lines[0].Length;
            var centerX = (w - 1) / 2;
            var centerY = (h - 1) / 2;

            Debug.Assert(w == 3 && h==3, "Only 3x3 input is supported");

            for (int y = 0; y < h; y++)
            {
                Debug.Assert(lines[y].Length == w, "Non rectangualr input");
                for (int x = 0; x < w; x++)
                {
                    var c = lines[y][x];
                    if (c == '.')
                        continue;
                    var dir = new Int2(x - centerX, y - centerY).ToDirection();
                    result[(int)dir] = c;  
                }
            }
            return new DirectionTileRule(result);

        }
    }
}
