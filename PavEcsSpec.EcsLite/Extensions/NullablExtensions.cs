using System.Collections.Generic;

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

        internal static TV GetOrCreate<TK,TV>(this IDictionary<TK,TV> dict, TK key)
            where TV : new()
        {
            TV value;
            if (!dict.TryGetValue(key, out value))
            {
                value = new TV();
                dict.Add(key, value);
            }
            return value;
        }
    }
}