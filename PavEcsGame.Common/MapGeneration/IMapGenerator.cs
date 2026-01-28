using System.Threading.Tasks;

namespace PavEcsGame.MapGeneration
{
    /// <summary>
    /// Map generator interface.
    /// Takes initial state and mask, returns generated map.
    /// Mask indicates which cells to preserve (non-null char = preserve that cell from state).
    /// </summary>
    public interface IMapGenerator
    {
        /// <summary>
        /// Generate map.
        /// </summary>
        /// <param name="state">Initial state (provides dimensions and initial values)</param>
        /// <param name="mask">Mask where non-'\0' means preserve cell from state. Can be null.</param>
        /// <returns>Generated map</returns>
        Task<MapData> GenerateAsync(MapData state, MapData mask = null);
    }
}
