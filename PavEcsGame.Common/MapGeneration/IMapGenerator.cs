using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Input for map generation
    /// </summary>
    public class MapGeneratorInput
    {
        /// <summary>
        /// Initial state of the map. If null, generator creates from scratch.
        /// </summary>
        public MapData InitialState { get; init; }

        /// <summary>
        /// Desired width of output (used when InitialState is null)
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// Desired height of output (used when InitialState is null)
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        /// Characters that should be preserved in output (not modified by generator)
        /// </summary>
        public HashSet<char> ConstrainedChars { get; init; } = new() { 'x', 'X' };

        /// <summary>
        /// Random seed for reproducible generation (null = random)
        /// </summary>
        public int? Seed { get; init; }

        /// <summary>
        /// Get effective width (from InitialState or explicit Width)
        /// </summary>
        public int EffectiveWidth => InitialState?.Width ?? Width;

        /// <summary>
        /// Get effective height (from InitialState or explicit Height)
        /// </summary>
        public int EffectiveHeight => InitialState?.Height ?? Height;

        public static MapGeneratorInput FromSize(int width, int height) =>
            new() { Width = width, Height = height };

        public static MapGeneratorInput FromInitialState(MapData state) =>
            new() { InitialState = state };
    }

    /// <summary>
    /// Result of map generation
    /// </summary>
    public class MapGeneratorOutput
    {
        /// <summary>
        /// Generated map data
        /// </summary>
        public MapData Data { get; init; }

        /// <summary>
        /// Whether generation was successful
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Error message if generation failed
        /// </summary>
        public string Error { get; init; }

        /// <summary>
        /// Metadata about the generation (zone info, iterations, etc.)
        /// </summary>
        public Dictionary<string, object> Metadata { get; init; } = new();

        public static MapGeneratorOutput Successful(MapData data) =>
            new() { Data = data, Success = true };

        public static MapGeneratorOutput Failed(string error) =>
            new() { Success = false, Error = error };
    }

    /// <summary>
    /// Abstract map generator interface
    /// </summary>
    public interface IMapGenerator
    {
        /// <summary>
        /// Generate a map based on input
        /// </summary>
        Task<MapGeneratorOutput> GenerateAsync(MapGeneratorInput input);
    }

    /// <summary>
    /// Extension methods for IMapGenerator
    /// </summary>
    public static class MapGeneratorExtensions
    {
        /// <summary>
        /// Generate with simple size parameters
        /// </summary>
        public static Task<MapGeneratorOutput> GenerateAsync(this IMapGenerator generator, int width, int height) =>
            generator.GenerateAsync(MapGeneratorInput.FromSize(width, height));

        /// <summary>
        /// Generate from initial state
        /// </summary>
        public static Task<MapGeneratorOutput> GenerateAsync(this IMapGenerator generator, MapData initialState) =>
            generator.GenerateAsync(MapGeneratorInput.FromInitialState(initialState));

        /// <summary>
        /// Chain generators: output of first becomes input of second
        /// </summary>
        public static IMapGenerator Then(this IMapGenerator first, IMapGenerator second) =>
            new ChainedMapGenerator(first, second);
    }

    /// <summary>
    /// Chains two generators sequentially
    /// </summary>
    internal class ChainedMapGenerator : IMapGenerator
    {
        private readonly IMapGenerator _first;
        private readonly IMapGenerator _second;

        public ChainedMapGenerator(IMapGenerator first, IMapGenerator second)
        {
            _first = first;
            _second = second;
        }

        public async Task<MapGeneratorOutput> GenerateAsync(MapGeneratorInput input)
        {
            var firstResult = await _first.GenerateAsync(input);
            if (!firstResult.Success)
                return firstResult;

            var secondInput = new MapGeneratorInput
            {
                InitialState = firstResult.Data,
                ConstrainedChars = input.ConstrainedChars,
                Seed = input.Seed
            };

            return await _second.GenerateAsync(secondInput);
        }
    }
}
