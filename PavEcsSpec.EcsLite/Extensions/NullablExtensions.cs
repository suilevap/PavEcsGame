namespace PavEcsSpec.EcsLite
{
    internal static class NullableExtensions
    {
        internal static bool TryGet<T>(this T? item, out T value)  where T: struct
        {
            if (item.HasValue)
            {
                value = item.Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        } 
    }
}