using System.Threading.Tasks;
using PavEcsGame.Components;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Map generator interface.
    /// </summary>
    public interface IMapGenerator
    {
        /// <summary>
        /// Generate map content.
        /// </summary>
        /// <param name="state">Initial state (provides dimensions and initial values)</param>
        /// <param name="mask">Optional mask - non-default values mean "preserve from state"</param>
        Task<MapData<char>> GenerateAsync(MapData<char> state, MapData<bool> mask = null);
    }
}
