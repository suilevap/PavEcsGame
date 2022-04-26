using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsSpec.Generators
{
    internal static class TypeToWorldNameGenerator
    {
        public static string GeneratedCode(Dictionary<string, Dictionary<ITypeSymbol, string>> wolrdNameMap)
        {
            var universeSwitch = new StringBuilder();
            var mapsCode = new StringBuilder();

            foreach (var pair in wolrdNameMap)
            {
                var universeName = pair.Key;
                var fieldName = !string.IsNullOrEmpty(universeName) ? universeName : "Default";

                universeSwitch.AppendLine($@"case ""{universeName ?? String.Empty}"" :");
                universeSwitch.AppendLine($@"   return {fieldName};");
                StringBuilder mapping = new StringBuilder();
                foreach (var typeAndWorld in pair.Value)
                {
                    var type = typeAndWorld.Key;

                    if (type.DeclaredAccessibility == Accessibility.Private 
                        || type.ContainingType?.DeclaredAccessibility == Accessibility.Private) //todo: do we need to check more levels?
                        continue;
                    if (type is ITypeParameterSymbol)
                        continue;
                    
                    mapping.AppendLine($@"{{ typeof({typeAndWorld.Key}),""{typeAndWorld.Value}"" }},");
                }
                mapsCode.AppendLine($@"
public static TypeToWorldNameMap {fieldName} = new TypeToWorldNameMap()
{{
    _map = new Dictionary<Type, string>()
    {{
{mapping.ToString().PadLeftAllLines(4 * 2)}
    }}
}};");
            }
            return $@"
#nullable disable
using System;
using System.Collections.Generic;

namespace PavEcsSpec.Generated
{{

    public partial class TypeToWorldNameMap
    {{
        public static partial string GetWorldName<T>(string universeName) where T : struct
        {{
            return GetMap(universeName).GetWorld<T>();
        }}

        private static TypeToWorldNameMap GetMap(string universeName)
        {{
            switch(universeName)
            {{
{universeSwitch.ToString().PadLeftAllLines(4 * 4)}
                default:
                    throw new ArgumentException($""Unknown universe {{universeName}}"");
            }}
        }}

{mapsCode.ToString().PadLeftAllLines(4 * 2)}" +
        (wolrdNameMap.Count != 0 ? @"
        private Dictionary<Type, string> _map;
        public string GetWorld<T>() where T : struct => _map[typeof(T)];

        " : @"
        public string GetWorld<T>() where T : struct => null;") +
        @$"
    }}
}}";
        }
    }
}
