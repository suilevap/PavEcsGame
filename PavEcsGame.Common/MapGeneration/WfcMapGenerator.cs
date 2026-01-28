using System.IO;
using System.Threading.Tasks;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// WFC-based map generator using DeBroglie
    /// </summary>
    public class WfcMapGenerator : IMapGenerator
    {
        private readonly MapData _pattern;
        private readonly int _patternSize;
        private readonly int _maxIterations;

        public WfcMapGenerator(MapData pattern, int patternSize = 2, int maxIterations = 100)
        {
            _pattern = pattern;
            _patternSize = patternSize;
            _maxIterations = maxIterations;
        }

        public static async Task<WfcMapGenerator> FromFileAsync(string path, int patternSize = 2)
        {
            var pattern = await MapData.LoadAsync(path);
            return new WfcMapGenerator(pattern, patternSize);
        }

        public Task<MapData> GenerateAsync(MapData state, MapData mask = null)
        {
            var width = state.Width;
            var height = state.Height;

            // Create model from pattern
            var sample = TopoArray.Create(_pattern.ToCharArray(), false);
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
            var result = state.Clone();

            if (status == Resolution.Decided)
            {
                var output = propagator.ToValueArray<char>();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Check mask - if mask cell is non-null, preserve state
                        if (mask != null && mask[x, y] != '\0')
                            continue;

                        result[x, y] = output.Get(x, y);
                    }
                }
            }

            return Task.FromResult(result);
        }
    }
}
