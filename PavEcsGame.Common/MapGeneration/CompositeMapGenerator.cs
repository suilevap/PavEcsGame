using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Composite generator that identifies zones in input state and fills each with zone-specific generator.
    /// Zone markers are uppercase letters (A-Z) except 'X' which is treated as wall.
    /// </summary>
    public class CompositeMapGenerator : IMapGenerator
    {
        private readonly Dictionary<char, IMapGenerator> _zoneGenerators;
        private readonly char _defaultFill;

        public CompositeMapGenerator(Dictionary<char, IMapGenerator> zoneGenerators, char defaultFill = '.')
        {
            _zoneGenerators = zoneGenerators ?? new Dictionary<char, IMapGenerator>();
            _defaultFill = defaultFill;
        }

        public async Task<MapData> GenerateAsync(MapData state, MapData mask = null)
        {
            var result = state.Clone();
            var width = state.Width;
            var height = state.Height;

            // Find zone bounds
            var zoneBounds = FindZoneBounds(state);

            // Process each zone
            foreach (var (marker, bounds) in zoneBounds)
            {
                if (!_zoneGenerators.TryGetValue(marker, out var generator))
                {
                    // No generator - fill with default
                    FillZone(result, marker, _defaultFill);
                    continue;
                }

                // Create zone state and mask
                var zoneState = new MapData(bounds.W, bounds.H, _defaultFill);
                var zoneMask = new MapData(bounds.W, bounds.H, '\0');

                for (int y = 0; y < bounds.H; y++)
                {
                    for (int x = 0; x < bounds.W; x++)
                    {
                        var sx = bounds.X + x;
                        var sy = bounds.Y + y;
                        var c = state[sx, sy];

                        if (c == marker)
                        {
                            // This cell is part of zone - will be generated
                            zoneState[x, y] = _defaultFill;
                        }
                        else
                        {
                            // Not part of zone - preserve
                            zoneState[x, y] = c;
                            zoneMask[x, y] = 'x'; // Mark as preserved
                        }
                    }
                }

                // Generate zone content
                var zoneResult = await generator.GenerateAsync(zoneState, zoneMask);

                // Copy back only zone cells
                for (int y = 0; y < bounds.H; y++)
                {
                    for (int x = 0; x < bounds.W; x++)
                    {
                        var sx = bounds.X + x;
                        var sy = bounds.Y + y;

                        if (state[sx, sy] == marker)
                        {
                            result[sx, sy] = zoneResult[x, y];
                        }
                    }
                }
            }

            return result;
        }

        private Dictionary<char, Bounds> FindZoneBounds(MapData map)
        {
            var bounds = new Dictionary<char, Bounds>();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var c = map[x, y];
                    if (!IsZoneMarker(c)) continue;

                    if (!bounds.TryGetValue(c, out var b))
                    {
                        b = new Bounds { X = x, Y = y, MaxX = x, MaxY = y };
                        bounds[c] = b;
                    }
                    else
                    {
                        b.X = Math.Min(b.X, x);
                        b.Y = Math.Min(b.Y, y);
                        b.MaxX = Math.Max(b.MaxX, x);
                        b.MaxY = Math.Max(b.MaxY, y);
                    }
                }
            }

            return bounds;
        }

        private static bool IsZoneMarker(char c) => c >= 'A' && c <= 'Z' && c != 'X';

        private static void FillZone(MapData map, char marker, char fill)
        {
            for (int y = 0; y < map.Height; y++)
            for (int x = 0; x < map.Width; x++)
                if (map[x, y] == marker)
                    map[x, y] = fill;
        }

        private class Bounds
        {
            public int X, Y, MaxX, MaxY;
            public int W => MaxX - X + 1;
            public int H => MaxY - Y + 1;
        }
    }
}
