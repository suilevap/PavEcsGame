using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace PavEcsSpec.Generators
{
    [Generator]
    public class SystemSetGenerator : ISourceGenerator
    {


        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization(
                c =>
                {
                    EcsInfraTypes.AddSources(c);
                });

            // Register a syntax receiver that will be created for each generation pass
            //context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

            context.RegisterForSyntaxNotifications(() => new EntitySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
#endif 
            try
            {
                ExecuteImpl(context);
            }
            catch (Exception ex)
            {
#if DEBUG
                //if (!Debugger.IsAttached)
                //{
                //    Debugger.Launch();
                //}
#endif 
                throw;
            }
        }

        private void ExecuteImpl(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 

            if (context.SyntaxReceiver is EntitySyntaxReceiver receiver)
            {
                var provider = new EntityProviderGenerator();


                var entityDescrs = receiver.Candidates
                    .Select(
                    declaration =>
                    {
                        var model = context.Compilation.GetSemanticModel(declaration.SyntaxTree, true);
                        var symbol = model.GetDeclaredSymbol(declaration);
                        if (symbol is ITypeSymbol type)
                        {
                            //if (type is null || !IsEnumeration(type))
                            //    continue;
                            try
                            {
                                var entityDescr = EcsEntityDescriptor.Create(type, declaration);
                                ReportDiagnostic(context, entityDescr, declaration);
                                return entityDescr;
                            }
                            catch(Exception ex)
                            {
                                ReportError(context, declaration, $"Ex {symbol.Name} {ex.Message} {ex.StackTrace}");
                            }
                        }
                        else
                        {
                            ReportError(context, declaration, $"unsupported symbol {symbol.Name}");
                        }
                        return default(EcsEntityDescriptor);
                    })
                    .Where(x => x != null)
                    .ToList();

                Dictionary<string, QuickUnionFind<ITypeSymbol>> universes = new Dictionary<string, QuickUnionFind<ITypeSymbol>>();
                foreach (var entity in entityDescrs)
                {
                    var universe = universes.GetOrCreate(entity.Universe);
                    universe.Union(entity.Components.Select(x => x.ComponentType)
                        .Concat(new[] { entity.EntityType })
                        .Concat(entity.BaseEntities.Select(x => x.EntityType)));
                }
                Dictionary<string, Dictionary<ITypeSymbol, string>> typeToWorldName =
                    new Dictionary<string, Dictionary<ITypeSymbol, string>>();
                foreach (var universe in universes)
                {
                    Dictionary<ITypeSymbol, string> map = new();
                    foreach (var gr in universe.Value.GetAllGroups())
                    {
                        foreach (var type in gr)
                        {
                            var worldName = $"GENERATED_{universe.Key}_{gr.Key}";
                            map[type] = worldName;
                        }
                    }
                    typeToWorldName[universe.Key] = map;
                }
                var mapCode = TypeToWorldNameGenerator.GeneratedCode(typeToWorldName);
                AddSource(context, $"{nameof(EcsInfraTypes.TypeToWorldNameMap)}.generated.cs", mapCode);


                Dictionary<ITypeSymbol, string> generatedCode = new Dictionary<ITypeSymbol, string>(SymbolEqualityComparer.IncludeNullability);
                foreach (var entity in entityDescrs)
                {
                    try
                    {
                        var worldName = typeToWorldName[entity.Universe][entity.Components.First().ComponentType];
                        var code = provider.GenerateEntityCode(entity, worldName);
                        generatedCode.Add(entity.EntityType, code);
                    }
                    catch (Exception ex)
                    {
                        ReportError(context, entity.Declaration, $"Ex {entity.EntityType} {ex.Message} {ex.StackTrace}");
                    }
                }
                foreach (var gr in entityDescrs.GroupBy(x => x.EntityType.ContainingType, SymbolEqualityComparer.IncludeNullability))
                {
                    if (gr.Key is ITypeSymbol parentType)
                    {
                        try
                        {
                            var providerCode = ProvidersGenerator.GenerateCode(gr);
                            generatedCode.Add(parentType, providerCode);
                        }
                        catch (Exception ex)
                        {
                            ReportError(context, gr.FirstOrDefault().Declaration, $"Ex {parentType.Name} {ex.Message} {ex.StackTrace}");
                        }
                    }
                }

                var types = NestedTypeGenerator.WrapNestedTypes(generatedCode);
                foreach (var pair in types)
                {
                    var fileName = $"{pair.Key.Name}.generated.cs";
                    var code = pair.Value;
                    AddSource(context, fileName, code);
                }

            }
        }

        private static void AddSource(GeneratorExecutionContext context, string fileName, string code)
        {
            context.AddSource(fileName, code);

            try
            {
                System.IO.File.WriteAllText("C:/dev/roslyn/" + fileName, code);
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(
                   Diagnostic.Create(
                       new DiagnosticDescriptor(
                           "0",
                           $"Entity {fileName}",
                          "Failed to save",
                           "EcsGenerator",
                           DiagnosticSeverity.Warning,
                           true),
                       null)
                   );
            }
        }

        private void ReportDiagnostic(GeneratorExecutionContext context, EcsEntityDescriptor entityDescr, StructDeclarationSyntax declaration)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "0",
                        $"Entity {entityDescr.EntityType}",
                        entityDescr.ToString().Replace(Environment.NewLine, "|"),
                        "EcsGenerator",
                        DiagnosticSeverity.Warning,
                        true),
                    declaration.GetLocation())
                );
        }
        private void ReportError(GeneratorExecutionContext context, StructDeclarationSyntax declaration, string error)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "0",
                        $"Error",
                        error,
                        "EcsGenerator",
                        DiagnosticSeverity.Error,
                        true),
                    declaration.GetLocation())
                );
        }

        class EntitySyntaxReceiver : ISyntaxReceiver
        {
            public List<StructDeclarationSyntax> Candidates { get; } = new List<StructDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is not AttributeSyntax attribute)
                    return;

                var name = ExtractName(attribute.Name);

                if (name != "Entity" && name != "EntityAttribute")
                    return;

                // "attribute.Parent" is "AttributeListSyntax"
                // "attribute.Parent.Parent" is a C# fragment the attribute is applied to
                if (attribute.Parent?.Parent is StructDeclarationSyntax declaration)
                    Candidates.Add(declaration);
            }

            private static string ExtractName(TypeSyntax type)
            {
                while (type != null)
                {
                    switch (type)
                    {
                        case IdentifierNameSyntax ins:
                            return ins.Identifier.Text;

                        case QualifiedNameSyntax qns:
                            type = qns.Right;
                            break;

                        default:
                            return null;
                    }
                }

                return null;
            }

        }
    }
}
