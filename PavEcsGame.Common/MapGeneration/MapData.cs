using System;
using System.IO;
using System.Threading.Tasks;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Simple 2D character grid
    /// </summary>
    public class MapData
    {
        private readonly char[,] _data;

        public int Width { get; }
        public int Height { get; }

        public MapData(int width, int height, char fill = '.')
        {
            Width = width;
            Height = height;
            _data = new char[height, width];
            Fill(fill);
        }

        public char this[int x, int y]
        {
            get => InBounds(x, y) ? _data[y, x] : '\0';
            set { if (InBounds(x, y)) _data[y, x] = value; }
        }

        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        public void Fill(char c)
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _data[y, x] = c;
        }

        public MapData Clone()
        {
            var clone = new MapData(Width, Height);
            Array.Copy(_data, clone._data, _data.Length);
            return clone;
        }

        public char[][] ToCharArray()
        {
            var result = new char[Height][];
            for (int y = 0; y < Height; y++)
            {
                result[y] = new char[Width];
                for (int x = 0; x < Width; x++)
                    result[y][x] = _data[y, x];
            }
            return result;
        }

        public string[] ToStringArray()
        {
            var result = new string[Height];
            for (int y = 0; y < Height; y++)
            {
                var chars = new char[Width];
                for (int x = 0; x < Width; x++)
                    chars[x] = _data[y, x];
                result[y] = new string(chars);
            }
            return result;
        }

        public static MapData FromLines(string[] lines)
        {
            if (lines == null || lines.Length == 0)
                return new MapData(0, 0);

            int height = lines.Length;
            int width = 0;
            foreach (var line in lines)
                if (line.Length > width) width = line.Length;

            var map = new MapData(width, height);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < lines[y].Length; x++)
                map[x, y] = lines[y][x];

            return map;
        }

        public static async Task<MapData> LoadAsync(string path)
        {
            var lines = await File.ReadAllLinesAsync(path);
            return FromLines(lines);
        }
    }
}
