using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;
using Leopotam.EcsLite;
using PavEcsGame.Components.Events;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    /// <summary>
    /// Hierarchical map generator that uses WFC in two passes:
    /// 1. Blueprint pass: Defines zone boundaries using zone markers (A, B, C, D, E...)
    /// 2. Detail pass: Fills each zone with zone-specific WFC patterns
    /// </summary>
    internal partial class HierarchicalMapGeneratorSystem : IEcsRunSystem
    {
        [Entity(SkipFilter = true)]
        private readonly partial struct MapDataEnt
        {
            public partial ref MapRawDataEvent Event();
        }

        /// <summary>
        /// Configuration for zone-to-pattern mapping
        /// </summary>
        public class ZoneConfig
        {
            public char ZoneMarker { get; set; }
            public string PatternFile { get; set; }
            public int PatternSize { get; set; } = 2;
        }

        /// <summary>
        /// Configuration for hierarchical generation
        /// </summary>
        public class GenerationConfig
        {
            public string BlueprintFile { get; set; }
            public List<ZoneConfig> Zones { get; set; } = new();
            public int MaxIterations { get; set; } = 100;
        }

        /// <summary>
        /// Generates a map using hierarchical WFC approach
        /// </summary>
        /// <param name="config">Generation configuration</param>
        public async void GenerateHierarchicalMap(GenerationConfig config)
        {
            // Load blueprint
            var blueprintLines = await File.ReadAllLinesAsync(config.BlueprintFile);
            if (blueprintLines == null || blueprintLines.Length == 0)
                return;

            var width = blueprintLines.Max(l => l.Length);
            var height = blueprintLines.Length;

            // Normalize blueprint lines to same width
            var blueprint = new char[height][];
            for (int y = 0; y < height; y++)
            {
                blueprint[y] = new char[width];
                for (int x = 0; x < width; x++)
                {
                    blueprint[y][x] = x < blueprintLines[y].Length ? blueprintLines[y][x] : '.';
                }
            }

            // Load zone patterns
            var zonePatterns = new Dictionary<char, ITopoArray<Tile>>();
            var zonePatternSizes = new Dictionary<char, int>();

            foreach (var zone in config.Zones)
            {
                var patternLines = await File.ReadAllLinesAsync(zone.PatternFile);
                var patternChars = patternLines.Select(x => x.ToArray()).ToArray();
                var sample = TopoArray.Create(patternChars, false);
                zonePatterns[zone.ZoneMarker] = sample.ToTiles();
                zonePatternSizes[zone.ZoneMarker] = zone.PatternSize;
            }

            // Find all unique zones and their bounding boxes
            var zoneBounds = FindZoneBounds(blueprint, width, height);

            // Generate detail for each zone
            var output = new char[height][];
            for (int y = 0; y < height; y++)
            {
                output[y] = new char[width];
                Array.Copy(blueprint[y], output[y], width);
            }

            foreach (var (zoneMarker, bounds) in zoneBounds)
            {
                if (!zonePatterns.TryGetValue(zoneMarker, out var pattern))
                    continue;

                var patternSize = zonePatternSizes.GetValueOrDefault(zoneMarker, 2);
                var zoneOutput = GenerateZoneContent(
                    pattern,
                    bounds.Width,
                    bounds.Height,
                    patternSize,
                    config.MaxIterations);

                if (zoneOutput != null)
                {
                    // Copy zone output to main output
                    for (int y = 0; y < bounds.Height; y++)
                    {
                        for (int x = 0; x < bounds.Width; x++)
                        {
                            var outX = bounds.X + x;
                            var outY = bounds.Y + y;
                            if (outX < width && outY < height)
                            {
                                // Only replace zone markers, preserve walls and corridors
                                if (output[outY][outX] == zoneMarker)
                                {
                                    output[outY][outX] = zoneOutput.Get(x, y);
                                }
                            }
                        }
                    }
                }
            }

            // Convert corridors (C) to empty space
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (char.IsUpper(output[y][x]) && output[y][x] != 'X')
                    {
                        output[y][x] = '.';
                    }
                }
            }

            // Create output lines
            var outputLines = output.Select(row => new string(row)).ToArray();
            CreateMapDataComponent(config.BlueprintFile, outputLines);
        }

        /// <summary>
        /// Simplified generation using predefined zone configurations
        /// </summary>
        public async void GenerateMap(string blueprintFile, string dataFolder)
        {
            var config = new GenerationConfig
            {
                BlueprintFile = blueprintFile,
                Zones = new List<ZoneConfig>
                {
                    new() { ZoneMarker = 'A', PatternFile = Path.Combine(dataFolder, "wcf_zone_arena.txt") },
                    new() { ZoneMarker = 'B', PatternFile = Path.Combine(dataFolder, "wcf_zone_tactical.txt") },
                    new() { ZoneMarker = 'C', PatternFile = Path.Combine(dataFolder, "wcf_zone_scattered.txt") },
                    new() { ZoneMarker = 'D', PatternFile = Path.Combine(dataFolder, "wcf_zone_ruins.txt") },
                    new() { ZoneMarker = 'E', PatternFile = Path.Combine(dataFolder, "wcf_zone_scattered.txt") },
                }
            };

            GenerateHierarchicalMap(config);
        }

        private ITopoArray<char> GenerateZoneContent(
            ITopoArray<Tile> pattern,
            int width,
            int height,
            int patternSize,
            int maxIterations)
        {
            try
            {
                var model = new OverlappingModel(patternSize);
                model.AddSample(pattern);

                var topology = new GridTopology(width, height, false);
                var propagator = new TilePropagator(model, topology);

                var status = propagator.Run();
                var iterations = 0;
                while (iterations++ < maxIterations && status != Resolution.Decided)
                {
                    status = propagator.Run();
                }

                if (status == Resolution.Decided)
                {
                    return propagator.ToValueArray<char>();
                }
            }
            catch (Exception)
            {
                // If WFC fails, return null and keep original
            }

            return null;
        }

        private Dictionary<char, ZoneBounds> FindZoneBounds(char[][] blueprint, int width, int height)
        {
            var bounds = new Dictionary<char, ZoneBounds>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var c = blueprint[y][x];
                    // Zone markers are uppercase letters except X (wall)
                    if (char.IsUpper(c) && c != 'X')
                    {
                        if (!bounds.TryGetValue(c, out var zoneBounds))
                        {
                            zoneBounds = new ZoneBounds
                            {
                                X = x,
                                Y = y,
                                MaxX = x,
                                MaxY = y
                            };
                            bounds[c] = zoneBounds;
                        }
                        else
                        {
                            zoneBounds.X = Math.Min(zoneBounds.X, x);
                            zoneBounds.Y = Math.Min(zoneBounds.Y, y);
                            zoneBounds.MaxX = Math.Max(zoneBounds.MaxX, x);
                            zoneBounds.MaxY = Math.Max(zoneBounds.MaxY, y);
                        }
                    }
                }
            }

            return bounds;
        }

        private class ZoneBounds
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int MaxX { get; set; }
            public int MaxY { get; set; }
            public int Width => MaxX - X + 1;
            public int Height => MaxY - Y + 1;
        }

        private void CreateMapDataComponent(string fileName, string[] lines)
        {
            ref var data = ref _providers.MapDataEntProvider.New().Event();
            data.Name = fileName;
            data.Data = lines;
        }

        public void Run(IEcsSystems systems)
        {
            // This system is triggered manually via GenerateMap/GenerateHierarchicalMap
        }
    }
}
