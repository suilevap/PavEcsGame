using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Composite generator that uses a blueprint generator to define zones,
    /// then fills each zone with zone-specific generators
    /// </summary>
    public class CompositeMapGenerator : IMapGenerator
    {
        private readonly IMapGenerator _blueprintGenerator;
        private readonly Dictionary<char, IMapGenerator> _zoneGenerators;
        private readonly CompositeGeneratorConfig _config;

        public CompositeMapGenerator(
            IMapGenerator blueprintGenerator,
            Dictionary<char, IMapGenerator> zoneGenerators,
            CompositeGeneratorConfig config = null)
        {
            _blueprintGenerator = blueprintGenerator ?? throw new ArgumentNullException(nameof(blueprintGenerator));
            _zoneGenerators = zoneGenerators ?? throw new ArgumentNullException(nameof(zoneGenerators));
            _config = config ?? new CompositeGeneratorConfig();
        }

        public async Task<MapGeneratorOutput> GenerateAsync(MapGeneratorInput input)
        {
            // Step 1: Generate blueprint
            var blueprintResult = await _blueprintGenerator.GenerateAsync(input);
            if (!blueprintResult.Success)
            {
                return MapGeneratorOutput.Failed($"Blueprint generation failed: {blueprintResult.Error}");
            }

            var blueprint = blueprintResult.Data;

            // Step 2: Find all zones and their bounds
            var zoneBounds = FindZoneBounds(blueprint);

            // Step 3: Create output starting from blueprint
            var output = blueprint.Clone();

            // Step 4: Generate content for each zone
            var zoneResults = new Dictionary<char, MapGeneratorOutput>();

            foreach (var (zoneMarker, bounds) in zoneBounds)
            {
                if (!_zoneGenerators.TryGetValue(zoneMarker, out var zoneGenerator))
                {
                    // No generator for this zone - use default behavior
                    if (_config.DefaultGenerator != null)
                    {
                        zoneGenerator = _config.DefaultGenerator;
                    }
                    else
                    {
                        // Replace zone marker with empty space
                        FillZone(output, zoneMarker, _config.DefaultFillChar);
                        continue;
                    }
                }

                // Extract zone area for generator input
                var zoneInput = new MapGeneratorInput
                {
                    Width = bounds.Width,
                    Height = bounds.Height,
                    InitialState = _config.PassBlueprintToZones
                        ? ExtractZoneWithMarker(blueprint, bounds, zoneMarker)
                        : null,
                    ConstrainedChars = input.ConstrainedChars,
                    Seed = input.Seed
                };

                var zoneResult = await zoneGenerator.GenerateAsync(zoneInput);
                zoneResults[zoneMarker] = zoneResult;

                if (zoneResult.Success)
                {
                    // Blit zone result back to output
                    BlitZone(output, zoneResult.Data, bounds, zoneMarker, blueprint);
                }
                else if (_config.FailOnZoneError)
                {
                    return MapGeneratorOutput.Failed(
                        $"Zone '{zoneMarker}' generation failed: {zoneResult.Error}");
                }
                else
                {
                    // Fill failed zone with default
                    FillZone(output, zoneMarker, _config.DefaultFillChar);
                }
            }

            return new MapGeneratorOutput
            {
                Data = output,
                Success = true,
                Metadata = new Dictionary<string, object>
                {
                    ["zones"] = zoneBounds.Keys.ToList(),
                    ["zoneBounds"] = zoneBounds,
                    ["zoneResults"] = zoneResults
                }
            };
        }

        private Dictionary<char, ZoneBounds> FindZoneBounds(MapData map)
        {
            var bounds = new Dictionary<char, ZoneBounds>();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var c = map[x, y];

                    // Zone markers are identified by the predicate
                    if (!_config.IsZoneMarker(c))
                        continue;

                    if (!bounds.TryGetValue(c, out var zoneBounds))
                    {
                        zoneBounds = new ZoneBounds
                        {
                            MinX = x, MinY = y,
                            MaxX = x, MaxY = y
                        };
                        bounds[c] = zoneBounds;
                    }
                    else
                    {
                        zoneBounds.MinX = Math.Min(zoneBounds.MinX, x);
                        zoneBounds.MinY = Math.Min(zoneBounds.MinY, y);
                        zoneBounds.MaxX = Math.Max(zoneBounds.MaxX, x);
                        zoneBounds.MaxY = Math.Max(zoneBounds.MaxY, y);
                    }
                }
            }

            return bounds;
        }

        private MapData ExtractZoneWithMarker(MapData source, ZoneBounds bounds, char marker)
        {
            var result = new MapData(bounds.Width, bounds.Height);

            for (int y = 0; y < bounds.Height; y++)
            {
                for (int x = 0; x < bounds.Width; x++)
                {
                    var srcX = bounds.MinX + x;
                    var srcY = bounds.MinY + y;
                    var c = source[srcX, srcY];

                    // Only include cells that are part of this zone
                    result[x, y] = (c == marker) ? '.' : c;
                }
            }

            return result;
        }

        private void BlitZone(MapData output, MapData zoneData, ZoneBounds bounds, char marker, MapData blueprint)
        {
            for (int y = 0; y < bounds.Height; y++)
            {
                for (int x = 0; x < bounds.Width; x++)
                {
                    var outX = bounds.MinX + x;
                    var outY = bounds.MinY + y;

                    if (!output.IsInBounds(outX, outY))
                        continue;

                    // Only replace cells that were originally this zone marker
                    if (blueprint[outX, outY] == marker)
                    {
                        output[outX, outY] = zoneData[x, y];
                    }
                }
            }
        }

        private void FillZone(MapData output, char marker, char fillChar)
        {
            for (int y = 0; y < output.Height; y++)
            {
                for (int x = 0; x < output.Width; x++)
                {
                    if (output[x, y] == marker)
                    {
                        output[x, y] = fillChar;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Bounding box for a zone
    /// </summary>
    public class ZoneBounds
    {
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }

        public int Width => MaxX - MinX + 1;
        public int Height => MaxY - MinY + 1;
    }

    /// <summary>
    /// Configuration for composite generator
    /// </summary>
    public class CompositeGeneratorConfig
    {
        /// <summary>
        /// Predicate to identify zone marker characters.
        /// Default: uppercase letters except 'X'
        /// </summary>
        public Func<char, bool> IsZoneMarker { get; init; } =
            c => char.IsUpper(c) && c != 'X';

        /// <summary>
        /// Default fill character for zones without generators
        /// </summary>
        public char DefaultFillChar { get; init; } = '.';

        /// <summary>
        /// Default generator for zones without specific generators (null = use DefaultFillChar)
        /// </summary>
        public IMapGenerator DefaultGenerator { get; init; }

        /// <summary>
        /// Whether to pass blueprint section to zone generators as initial state
        /// </summary>
        public bool PassBlueprintToZones { get; init; } = false;

        /// <summary>
        /// Whether to fail entire generation if any zone fails
        /// </summary>
        public bool FailOnZoneError { get; init; } = false;
    }
}
