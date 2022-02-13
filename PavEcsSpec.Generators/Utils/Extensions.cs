using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsSpec.Generators
{
    internal static class Extensions
    {
        public static TV GetOrCreate<TK, TV>(this IDictionary<TK, TV> dict, TK key) where TV : new()
        {
            if (!dict.TryGetValue(key, out var result))
            {
                result = new TV();
                dict.Add(key, result);
            }
            return result;
        }

        public static bool IsRequired(this ComponentDescriptorAccessKind accessType) =>
          accessType == ComponentDescriptorAccessKind.Include || accessType == ComponentDescriptorAccessKind.IncludeReadonly;

    }
}
