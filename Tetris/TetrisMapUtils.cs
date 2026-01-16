using Tetris.Components;

namespace Tetris
{
    internal class TetrisMapUtils
    {
        public static bool IsFree(in MapComponent map, uint[] figure, int yOffset)
        {
            //var figureData = FiguresData.Instance.GetFigureData(figure.Shape, figure.Rotation);
            //var f4 = (figureData & 0b1111 << 16) >> 16;
            //var f3 = (figureData & 0b1111 << 8) >> 8;
            //var f2 = (figureData & 0b1111 << 4) >> 4;
            //var f1 = (figureData & 0b1111 << 0) >> 0;
            for (int i = 0; i < figure.Length; i++)
            {
                var f = figure[i];
                if (f == 0) continue;

                var y = yOffset + i;
                if (y < 0 || y >= map.Data.Length)
                {
                    return false;
                }
                if ((map.Data[y] & f) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static void Merge(in MapComponent map, uint[] figure, int yOffset)
        {
            for (int i = 0; i < figure.Length; i++)
            {
                var f = figure[i];
                var y = yOffset + i;
                map.Data[y] |= f;
            }
        }
        public static void FillFigureData(in Figure figure, int xOffset, ref uint[] data)
        {
            FillFigureData(figure.Shape, figure.Rotation, xOffset, ref data);
        }

        public static void FillFigureData(in FiguresData.FigureType figureType, byte rotation, int xOffset, ref uint[] data)
        {
            var figureData = FiguresData.Instance.GetFigureData(figureType, rotation);
            data[0] = ((figureData >> 12) & 0b1111) << (32 - 4 - xOffset);
            data[1] = ((figureData >> 8) & 0b1111) << (32 - 4 - xOffset);
            data[2] = ((figureData >> 4) & 0b1111) << (32 - 4 - xOffset);
            data[3] = ((figureData >> 0) & 0b1111) << (32 - 4 - xOffset);
        }
    }
}
