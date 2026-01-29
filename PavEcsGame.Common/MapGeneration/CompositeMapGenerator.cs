using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PavEcsGame.Components;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Composite generator that identifies zones in input and fills each with zone-specific generator.
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

        public async Task<MapData<char>> GenerateAsync(MapData<char> state, MapData<bool> mask = null)
        {
            var width = state.Width;
            var height = state.Height;

            // Copy state to result
            var result = new MapData<char>();
            result.Init(new Int2(width, height));
            result.CopyFrom(state);

            // Find zone bounds
            var zoneBounds = FindZoneBounds(state);

            // Process each zone
            foreach (var (marker, bounds) in zoneBounds)
            {
                if (!_zoneGenerators.TryGetValue(marker, out var generator))
                {
                    FillZone(result, marker, _defaultFill);
                    continue;
                }

                // Create zone state and mask
                var zoneState = new MapData<char>();
                zoneState.Init(new Int2(bounds.W, bounds.H));
                zoneState.Fill(_defaultFill);

                var zoneMask = new MapData<bool>();
                zoneMask.Init(new Int2(bounds.W, bounds.H));

                var pos = new Int2();
                for (pos.Y = 0; pos.Y < bounds.H; pos.Y++)
                {
                    for (pos.X = 0; pos.X < bounds.W; pos.X++)
                    {
                        var srcPos = new Int2(bounds.X + pos.X, bounds.Y + pos.Y);
                        var c = state.Get(srcPos);

                        if (c == marker)
                        {
                            zoneState.Set(pos, _defaultFill);
                        }
                        else
                        {
                            zoneState.Set(pos, c);
                            zoneMask.Set(pos, true); // Preserve non-zone cells
                        }
                    }
                }

                // Generate zone content
                var zoneResult = await generator.GenerateAsync(zoneState, zoneMask);

                // Copy back only zone cells
                for (pos.Y = 0; pos.Y < bounds.H; pos.Y++)
                {
                    for (pos.X = 0; pos.X < bounds.W; pos.X++)
                    {
                        var srcPos = new Int2(bounds.X + pos.X, bounds.Y + pos.Y);
                        if (state.Get(srcPos) == marker)
                        {
                            result.Set(srcPos, zoneResult.Get(pos));
                        }
                    }
                }
            }

            return result;
        }

        private Dictionary<char, Bounds> FindZoneBounds(MapData<char> map)
        {
            var bounds = new Dictionary<char, Bounds>();
            var pos = new Int2();

            for (pos.Y = 0; pos.Y < map.Height; pos.Y++)
            {
                for (pos.X = 0; pos.X < map.Width; pos.X++)
                {
                    var c = map.Get(pos);
                    if (!IsZoneMarker(c)) continue;

                    if (!bounds.TryGetValue(c, out var b))
                    {
                        b = new Bounds { X = pos.X, Y = pos.Y, MaxX = pos.X, MaxY = pos.Y };
                        bounds[c] = b;
                    }
                    else
                    {
                        b.X = Math.Min(b.X, pos.X);
                        b.Y = Math.Min(b.Y, pos.Y);
                        b.MaxX = Math.Max(b.MaxX, pos.X);
                        b.MaxY = Math.Max(b.MaxY, pos.Y);
                    }
                }
            }

            return bounds;
        }

        private static bool IsZoneMarker(char c) => c >= 'A' && c <= 'Z' && c != 'X';

        private static void FillZone(MapData<char> map, char marker, char fill)
        {
            var pos = new Int2();
            for (pos.Y = 0; pos.Y < map.Height; pos.Y++)
            for (pos.X = 0; pos.X < map.Width; pos.X++)
                if (map.Get(pos) == marker)
                    map.Set(pos, fill);
        }

        private class Bounds
        {
            public int X, Y, MaxX, MaxY;
            public int W => MaxX - X + 1;
            public int H => MaxY - Y + 1;
        }
    }
}
