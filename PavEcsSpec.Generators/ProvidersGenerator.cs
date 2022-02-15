using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsSpec.Generators
{
    internal static class ProvidersGenerator
    {
        public static string GenerateCode(IEnumerable<EcsEntityDescriptor> entities)
        {
            var props = new StringBuilder();
            var thisConstructor = new StringBuilder();
            var simpleConstructorSigntature = new StringBuilder();
            var simpleConstructorCode = new StringBuilder();

            foreach (var entity in entities)
            {
                if (thisConstructor.Length > 0)
                {
                    thisConstructor.Append(",");
                    simpleConstructorSigntature.Append(",");

                }
                var name = entity.EntityType.Name;
                props.AppendLine($"public {name}.Provider {name}Provider {{ get; }}");
                thisConstructor.Append($"{name}.Create(systems)");

                var argName = $"{name}Provider".ToLowerCaseFirstChar();
                simpleConstructorSigntature.Append($"{name}.Provider {argName}");
                simpleConstructorCode.AppendLine($"{name}Provider = {argName};");
            }

            return $@"
private readonly struct Providers
{{
{props.ToString().PadLeftAllLines(4*1)}

    public Providers(Leopotam.EcsLite.EcsSystems systems)
        : this({thisConstructor.ToString()})
    {{
    }}
    public Providers({simpleConstructorSigntature.ToString()})
    {{
        //Entity2Provider = entity2Provider;
{simpleConstructorCode.ToString().PadLeftAllLines(4*2)}
    }}
}}
";
        }
    }
}
