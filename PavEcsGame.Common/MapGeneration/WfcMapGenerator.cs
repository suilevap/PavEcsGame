using System.Threading.Tasks;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;
using PavEcsGame.Components;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// WFC-based map generator using DeBroglie
    /// </summary>
    public class WfcMapGenerator : IMapGenerator
    {
        private readonly MapData<char> _pattern;
        private readonly int _patternSize;
        private readonly int _maxIterations;

        public WfcMapGenerator(MapData<char> pattern, int patternSize = 2, int maxIterations = 100)
        {
            _pattern = pattern;
            _patternSize = patternSize;
            _maxIterations = maxIterations;
        }

        public Task<MapData<char>> GenerateAsync(MapData<char> state, MapData<bool> mask = null)
        {
            var width = state.Width;
            var height = state.Height;

            // Convert pattern to char[][]
            var patternArray = ToCharArray(_pattern);
            var sample = TopoArray.Create(patternArray, false);
            var tiles = sample.ToTiles();

            var model = new OverlappingModel(_patternSize);
            model.AddSample(tiles);

            // Run WFC
            var topology = new GridTopology(width, height, false);
            var propagator = new TilePropagator(model, topology);

            var status = propagator.Run();
            for (int i = 0; i < _maxIterations && status != Resolution.Decided; i++)
                status = propagator.Run();

            // Build result
            var result = new MapData<char>();
            result.Init(new Int2(width, height));

            if (status == Resolution.Decided)
            {
                var output = propagator.ToValueArray<char>();
                var pos = new Int2();
                for (pos.Y = 0; pos.Y < height; pos.Y++)
                {
                    for (pos.X = 0; pos.X < width; pos.X++)
                    {
                        if (mask != null && mask.Get(pos))
                        {
                            result.Set(pos, state.Get(pos));
                        }
                        else
                        {
                            result.Set(pos, output.Get(pos.X, pos.Y));
                        }
                    }
                }
            }
            else
            {
                result.CopyFrom(state);
            }

            return Task.FromResult(result);
        }

        private static char[][] ToCharArray(MapData<char> map)
        {
            var result = new char[map.Height][];
            for (int y = 0; y < map.Height; y++)
            {
                result[y] = new char[map.Width];
                for (int x = 0; x < map.Width; x++)
                    result[y][x] = map.Get(new Int2(x, y));
            }
            return result;
        }
    }
}
