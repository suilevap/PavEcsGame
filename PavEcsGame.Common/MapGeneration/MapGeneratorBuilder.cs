using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Builder for creating composite map generators with fluent API
    /// </summary>
    public class MapGeneratorBuilder
    {
        private IMapGenerator _blueprintGenerator;
        private readonly Dictionary<char, IMapGenerator> _zoneGenerators = new();
        private readonly CompositeGeneratorConfig _config = new();
        private string _dataFolder = "Data";

        /// <summary>
        /// Set the base data folder for pattern files
        /// </summary>
        public MapGeneratorBuilder WithDataFolder(string folder)
        {
            _dataFolder = folder;
            return this;
        }

        /// <summary>
        /// Set the blueprint generator
        /// </summary>
        public MapGeneratorBuilder WithBlueprint(IMapGenerator generator)
        {
            _blueprintGenerator = generator;
            return this;
        }

        /// <summary>
        /// Set blueprint from a pattern file
        /// </summary>
        public MapGeneratorBuilder WithBlueprintFile(string patternFile, int patternSize = 2)
        {
            var fullPath = Path.IsPathRooted(patternFile) ? patternFile : Path.Combine(_dataFolder, patternFile);
            _blueprintGenerator = new WfcMapGenerator(new WfcGeneratorConfig
            {
                PatternFile = fullPath,
                PatternSize = patternSize
            });
            return this;
        }

        /// <summary>
        /// Set blueprint from MapData (passthrough - no generation)
        /// </summary>
        public MapGeneratorBuilder WithBlueprintData(MapData blueprint)
        {
            _blueprintGenerator = new PassthroughGenerator(blueprint);
            return this;
        }

        /// <summary>
        /// Set blueprint from file content (passthrough - no generation)
        /// </summary>
        public MapGeneratorBuilder WithBlueprintFromFile(string filePath)
        {
            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(_dataFolder, filePath);
            _blueprintGenerator = new FilePassthroughGenerator(fullPath);
            return this;
        }

        /// <summary>
        /// Add a zone generator
        /// </summary>
        public MapGeneratorBuilder WithZone(char marker, IMapGenerator generator)
        {
            _zoneGenerators[marker] = generator;
            return this;
        }

        /// <summary>
        /// Add a zone with WFC generator from pattern file
        /// </summary>
        public MapGeneratorBuilder WithZonePattern(char marker, string patternFile, int patternSize = 2)
        {
            var fullPath = Path.IsPathRooted(patternFile) ? patternFile : Path.Combine(_dataFolder, patternFile);
            _zoneGenerators[marker] = new WfcMapGenerator(new WfcGeneratorConfig
            {
                PatternFile = fullPath,
                PatternSize = patternSize
            });
            return this;
        }

        /// <summary>
        /// Add multiple zones with same generator
        /// </summary>
        public MapGeneratorBuilder WithZones(string markers, IMapGenerator generator)
        {
            foreach (var marker in markers)
            {
                _zoneGenerators[marker] = generator;
            }
            return this;
        }

        /// <summary>
        /// Add multiple zones with same pattern file
        /// </summary>
        public MapGeneratorBuilder WithZonesPattern(string markers, string patternFile, int patternSize = 2)
        {
            var fullPath = Path.IsPathRooted(patternFile) ? patternFile : Path.Combine(_dataFolder, patternFile);
            var generator = new WfcMapGenerator(new WfcGeneratorConfig
            {
                PatternFile = fullPath,
                PatternSize = patternSize
            });

            foreach (var marker in markers)
            {
                _zoneGenerators[marker] = generator;
            }
            return this;
        }

        /// <summary>
        /// Build the composite generator
        /// </summary>
        public IMapGenerator Build()
        {
            if (_blueprintGenerator == null)
                throw new InvalidOperationException("Blueprint generator not set");

            return new CompositeMapGenerator(_blueprintGenerator, _zoneGenerators, _config);
        }

        /// <summary>
        /// Build and immediately generate a map
        /// </summary>
        public Task<MapGeneratorOutput> GenerateAsync(int width, int height)
        {
            return Build().GenerateAsync(width, height);
        }

        /// <summary>
        /// Build and immediately generate from initial state
        /// </summary>
        public Task<MapGeneratorOutput> GenerateAsync(MapData initialState)
        {
            return Build().GenerateAsync(initialState);
        }
    }

    /// <summary>
    /// Generator that just passes through initial state or preset data
    /// </summary>
    public class PassthroughGenerator : IMapGenerator
    {
        private readonly MapData _data;

        public PassthroughGenerator(MapData data = null)
        {
            _data = data;
        }

        public Task<MapGeneratorOutput> GenerateAsync(MapGeneratorInput input)
        {
            var result = _data?.Clone() ?? input.InitialState?.Clone();

            if (result == null)
            {
                result = new MapData(input.EffectiveWidth, input.EffectiveHeight);
            }

            return Task.FromResult(MapGeneratorOutput.Successful(result));
        }
    }

    /// <summary>
    /// Generator that loads data from file
    /// </summary>
    public class FilePassthroughGenerator : IMapGenerator
    {
        private readonly string _filePath;
        private MapData _cachedData;

        public FilePassthroughGenerator(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<MapGeneratorOutput> GenerateAsync(MapGeneratorInput input)
        {
            if (_cachedData == null)
            {
                var lines = await File.ReadAllLinesAsync(_filePath);
                _cachedData = new MapData(lines);
            }

            return MapGeneratorOutput.Successful(_cachedData.Clone());
        }
    }

    /// <summary>
    /// Static helper for quick generator creation
    /// </summary>
    public static class MapGenerator
    {
        /// <summary>
        /// Create a new builder
        /// </summary>
        public static MapGeneratorBuilder Create() => new();

        /// <summary>
        /// Create a simple WFC generator
        /// </summary>
        public static IMapGenerator Wfc(string patternFile, int patternSize = 2) =>
            new WfcMapGenerator(new WfcGeneratorConfig
            {
                PatternFile = patternFile,
                PatternSize = patternSize
            });

        /// <summary>
        /// Create a passthrough generator
        /// </summary>
        public static IMapGenerator Passthrough(MapData data = null) =>
            new PassthroughGenerator(data);

        /// <summary>
        /// Create a file-based passthrough generator
        /// </summary>
        public static IMapGenerator FromFile(string filePath) =>
            new FilePassthroughGenerator(filePath);
    }
}
