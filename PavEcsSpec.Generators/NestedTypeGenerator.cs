using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace PavEcsSpec.Generators
{
    internal class NestedTypeGenerator
    {
        public static Dictionary<ITypeSymbol, string> WrapNestedTypes(Dictionary<ITypeSymbol, string> generatedCode)
        {
            Dictionary<ITypeSymbol, StringBuilder> parents = new Dictionary<ITypeSymbol, StringBuilder>(SymbolEqualityComparer.IncludeNullability);

            foreach (KeyValuePair<ITypeSymbol, string> pair in generatedCode)
            {
                var parentType = pair.Key.ContainingType ?? pair.Key;
                if (!parents.TryGetValue(parentType, out var sb))
                {
                    sb = new StringBuilder();
                    parents.Add(parentType, sb);
                }

                sb.AppendLine(pair.Value);
            }

            Dictionary<ITypeSymbol, string> result = new Dictionary<ITypeSymbol, string>(SymbolEqualityComparer.IncludeNullability);


            foreach (KeyValuePair<ITypeSymbol, StringBuilder> pair in parents)
            {
                StringBuilder sb = new StringBuilder();
                var type = pair.Key;
             
                sb.Append($"partial {(type.IsReferenceType? "class": "struct")} {type.Name}" );
                if (type is INamedTypeSymbol namedType && namedType.Arity > 0)
                {

                    sb.AppendLine($"<{(string.Join(",",namedType.TypeParameters))}>");
                }
                sb.AppendLine(@"{");
                //todo intend
                sb.AppendLine(pair.Value.ToString().PadLeftAllLines(4));
                sb.AppendLine(@"}");

                if (type.ContainingType == null)
                {
                    StringBuilder finalSb = new StringBuilder();
                    var ns = type.ContainingNamespace.ToString();
                    //todo append usings
                    
                    finalSb.AppendLine(@"
using System;
using System.Runtime.CompilerServices;
");

                    finalSb.AppendLine((String.IsNullOrWhiteSpace(ns) ? null : $"namespace {ns}"));
                    finalSb.AppendLine(@"{");
                    finalSb.AppendLine(sb.ToString().PadLeftAllLines(4));
                    finalSb.AppendLine(@"}");
                    result.Add(type, finalSb.ToString());
                }
                else
                {
                    throw new InvalidOperationException(
                        $"currently do not support more than 1 level of nesting {type.Name}");
                }


                //         string SystemEmpty = @"
                //using System;
                //using System.Collections.Generic;
                //using System.Linq.Expressions;
                //using System.Runtime.CompilerServices;
                //using System.Text;
                //using GenerateTest;
                //using Leopotam.Ecs.Types;
                //using Leopotam.EcsLite;
                //using PavEcsGame.Components;
                //using PavEcsSpec.EcsLite;

                //namespace PavEcsGame.Systems
                //{
                //    partial class EmptySystem
                //    {
                //"
            }

            return result;
        }
        
    }

   
}
