namespace Tetris.Components
{
    internal struct MapComponent
    {
        public uint[] Data;
        public int Width { get; set; }

        public uint FullLine;
        public uint Border;
    }
}
