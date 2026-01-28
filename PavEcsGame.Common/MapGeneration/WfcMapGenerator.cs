using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// WFC-based map generator using DeBroglie library
    /// </summary>
    public class WfcMapGenerator : IMapGenerator
    {
        private readonly WfcGeneratorConfig _config;
        private ITopoArray<Tile> _patternTiles;

        public WfcMapGenerator(WfcGeneratorConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Create generator from pattern file
        /// </summary>
        public static async Task<WfcMapGenerator> FromFileAsync(string patternFile, int patternSize = 2)
        {
            var lines = await File.ReadAllLinesAsync(patternFile);
            var pattern = new MapData(lines);
            return FromPattern(pattern, patternSize);
        }

        /// <summary>
        /// Create generator from pattern MapData
        /// </summary>
        public static WfcMapGenerator FromPattern(MapData pattern, int patternSize = 2)
        {
            return new WfcMapGenerator(new WfcGeneratorConfig
            {
                Pattern = pattern,
                PatternSize = patternSize
            });
        }

        public async Task<MapGeneratorOutput> GenerateAsync(MapGeneratorInput input)
        {
            try
            {
                // Ensure pattern is loaded
                await EnsurePatternLoadedAsync();

                var width = input.EffectiveWidth;
                var height = input.EffectiveHeight;

                if (width <= 0 || height <= 0)
                {
                    return MapGeneratorOutput.Failed("Invalid dimensions");
                }

                // Create model
                var model = new OverlappingModel(_config.PatternSize);
                model.AddSample(_patternTiles);

                // Create topology
                var topology = new GridTopology(width, height, _config.Periodic);

                // Create propagator
                var propagator = new TilePropagator(model, topology);

                // Run WFC
                var status = propagator.Run();
                var iterations = 0;
                while (iterations++ < _config.MaxIterations && status != Resolution.Decided)
                {
                    status = propagator.Run();
                }

                if (status != Resolution.Decided)
                {
                    return MapGeneratorOutput.Failed($"WFC failed to converge after {iterations} iterations");
                }

                // Extract output
                var wfcOutput = propagator.ToValueArray<char>();
                var result = new MapData(width, height);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var generated = wfcOutput.Get(x, y);

                        // Check if this position should be constrained from initial state
                        if (input.InitialState != null)
                        {
                            var initial = input.InitialState[x, y];
                            if (input.ConstrainedChars.Contains(initial))
                            {
                                result[x, y] = initial;
                                continue;
                            }
                        }

                        result[x, y] = generated;
                    }
                }

                return MapGeneratorOutput.Successful(result);
            }
            catch (Exception ex)
            {
                return MapGeneratorOutput.Failed($"WFC generation error: {ex.Message}");
            }
        }

        private async Task EnsurePatternLoadedAsync()
        {
            if (_patternTiles != null)
                return;

            MapData pattern;

            if (_config.Pattern != null)
            {
                pattern = _config.Pattern;
            }
            else if (!string.IsNullOrEmpty(_config.PatternFile))
            {
                var lines = await File.ReadAllLinesAsync(_config.PatternFile);
                pattern = new MapData(lines);
            }
            else
            {
                throw new InvalidOperationException("No pattern specified");
            }

            var charArray = pattern.ToCharArray();
            var sample = TopoArray.Create(charArray, _config.Periodic);
            _patternTiles = sample.ToTiles();
        }
    }

    /// <summary>
    /// Configuration for WFC generator
    /// </summary>
    public class WfcGeneratorConfig
    {
        /// <summary>
        /// Pattern to learn from (either this or PatternFile must be set)
        /// </summary>
        public MapData Pattern { get; init; }

        /// <summary>
        /// Path to pattern file (either this or Pattern must be set)
        /// </summary>
        public string PatternFile { get; init; }

        /// <summary>
        /// Size of overlapping patterns (2 = 2x2, 3 = 3x3)
        /// </summary>
        public int PatternSize { get; init; } = 2;

        /// <summary>
        /// Maximum iterations before giving up
        /// </summary>
        public int MaxIterations { get; init; } = 100;

        /// <summary>
        /// Whether the output should wrap around
        /// </summary>
        public bool Periodic { get; init; } = false;
    }
}
