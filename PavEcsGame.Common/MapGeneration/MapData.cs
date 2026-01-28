using System;
using System.Text;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Represents a 2D map grid with character cells
    /// </summary>
    public class MapData
    {
        private readonly char[,] _data;

        public int Width { get; }
        public int Height { get; }

        public MapData(int width, int height, char fillChar = '.')
        {
            Width = width;
            Height = height;
            _data = new char[height, width];
            Fill(fillChar);
        }

        public MapData(char[,] data)
        {
            Height = data.GetLength(0);
            Width = data.GetLength(1);
            _data = (char[,])data.Clone();
        }

        public MapData(string[] lines)
        {
            if (lines == null || lines.Length == 0)
                throw new ArgumentException("Lines cannot be null or empty", nameof(lines));

            Height = lines.Length;
            Width = 0;
            foreach (var line in lines)
            {
                if (line.Length > Width) Width = line.Length;
            }

            _data = new char[Height, Width];
            Fill('.');

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    _data[y, x] = lines[y][x];
                }
            }
        }

        public char this[int x, int y]
        {
            get => IsInBounds(x, y) ? _data[y, x] : '\0';
            set
            {
                if (IsInBounds(x, y))
                    _data[y, x] = value;
            }
        }

        public bool IsInBounds(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;

        public void Fill(char c)
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _data[y, x] = c;
        }

        public void FillRect(int startX, int startY, int width, int height, char c)
        {
            for (int y = startY; y < startY + height && y < Height; y++)
            for (int x = startX; x < startX + width && x < Width; x++)
                if (IsInBounds(x, y))
                    _data[y, x] = c;
        }

        public MapData Clone() => new MapData((char[,])_data.Clone());

        /// <summary>
        /// Extract a rectangular region as a new MapData
        /// </summary>
        public MapData Extract(int startX, int startY, int width, int height)
        {
            var result = new MapData(width, height);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var srcX = startX + x;
                var srcY = startY + y;
                result[x, y] = IsInBounds(srcX, srcY) ? this[srcX, srcY] : '.';
            }
            return result;
        }

        /// <summary>
        /// Copy data from another MapData at specified position
        /// </summary>
        public void Blit(MapData source, int destX, int destY, Func<char, char, char> mergeFunc = null)
        {
            mergeFunc ??= (src, _) => src;

            for (int y = 0; y < source.Height; y++)
            for (int x = 0; x < source.Width; x++)
            {
                var dx = destX + x;
                var dy = destY + y;
                if (IsInBounds(dx, dy))
                {
                    this[dx, dy] = mergeFunc(source[x, y], this[dx, dy]);
                }
            }
        }

        public string[] ToStringArray()
        {
            var result = new string[Height];
            for (int y = 0; y < Height; y++)
            {
                var sb = new StringBuilder(Width);
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(_data[y, x]);
                }
                result[y] = sb.ToString();
            }
            return result;
        }

        public char[][] ToCharArray()
        {
            var result = new char[Height][];
            for (int y = 0; y < Height; y++)
            {
                result[y] = new char[Width];
                for (int x = 0; x < Width; x++)
                {
                    result[y][x] = _data[y, x];
                }
            }
            return result;
        }

        public override string ToString() => string.Join("\n", ToStringArray());
    }
}
