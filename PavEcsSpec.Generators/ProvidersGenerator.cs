using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PavEcsSpec.Generators
{

    internal static class ProvidersGenerator
    {
        public readonly struct Result
        {
            public string Code { get; }
            public bool HasSimpleCtor { get; }


            public Result(string code, bool hasSimpleCtor)
            {
                Code = code;
                HasSimpleCtor = hasSimpleCtor;
            }

        }

        public static Result GenerateCode(IEnumerable<EcsEntityDescriptor> entities)
        {
            var props = new StringBuilder();
            var thisConstructor = new StringBuilder();
            var simpleConstructorSigntature = new StringBuilder();
            var simpleConstructorCode = new StringBuilder();

            bool hasSimpleCtor = false;

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
            string defaultCtr = String.Empty;
            if (entities.All(x => !x.ExtraArgs.Any())
                && entities.All(x => x.Components.All(c => c.ComponentType is not ITypeParameterSymbol)))
            {
                hasSimpleCtor = true;
                defaultCtr = $@"
public Providers(Leopotam.EcsLite.EcsSystems systems)
    : this({thisConstructor.ToString()})
{{
}}
";
            }

            var code = $@"
private readonly struct Providers
{{
{props.ToString().PadLeftAllLines(4 * 1)}

{defaultCtr.PadLeftAllLines(4 * 1)}

    public Providers({simpleConstructorSigntature.ToString()})
    {{
{simpleConstructorCode.ToString().PadLeftAllLines(4 * 2)}
    }}
}}
";
            return new Result(code, hasSimpleCtor);
        }
    }
}
