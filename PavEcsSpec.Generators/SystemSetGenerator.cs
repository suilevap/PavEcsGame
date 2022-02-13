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
            // retrieve the populated receiver 
#if DEBUG
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
#endif 
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
                            var entityDescr = EcsEntityDescriptor.Create(type);
                            ReportDiagnostic(context, entityDescr, declaration);
                            return entityDescr;
                        }
                        else
                        {
                            throw new InvalidOperationException($"unsupported symbol {symbol.Name}");
                        }
                    })
                    .ToList();

                Dictionary<string, QuickUnionFind<ITypeSymbol>> universes = new Dictionary<string, QuickUnionFind<ITypeSymbol>>();
                foreach (var entity in entityDescrs)
                {
                    var universe = universes.GetOrCreate(entity.Universe);
                    universe.Union(entity.Components.Select(x => x.ComponentType));
                }
                
                Dictionary<ITypeSymbol, string> generatedCode = new Dictionary<ITypeSymbol, string>(SymbolEqualityComparer.IncludeNullability);
                foreach (var entity in entityDescrs)
                {
                    var universe = universes[entity.Universe];
                    var wolrdId = universe.Root(entity.Components.First().ComponentType);
                    var code = provider.GenerateEntityCode(entity, wolrdId);
                    generatedCode.Add(entity.EntityType, code);
                }
                //foreach (var declaration in receiver.Candidates)
                //{

                //    var model = context.Compilation.GetSemanticModel(declaration.SyntaxTree, true);
                //    if (model.GetDeclaredSymbol(declaration) is ITypeSymbol type)
                //    {
                //        //if (type is null || !IsEnumeration(type))
                //        //    continue;
                //        var entityDescr = EcsEntityDescriptor.Create(type);
                //        ReportDiagnostic(context, entityDescr, declaration);
                //        var code = provider.GenerateEntityCode(entityDescr);
                //        generatedCode.Add(type, code);
                //    }
                //}



                var types = NestedTypeGenerator.WrapNestedTypes(generatedCode);
                foreach (var pair in types)
                {
                    context.AddSource($"{pair.Key.Name}.generated.cs", pair.Value);
                }
                //context.AddSource($"{type.Name}_Generated.cs", code);

            }

            //var provider = new EntityProviderGenerator();
            //context.AddSource("EmptySystem.generated.cs", SourceText.From(provider.GetSource(), Encoding.UTF8));
        }

        private void ReportDiagnostic(GeneratorExecutionContext context, EcsEntityDescriptor entityDescr, StructDeclarationSyntax declaration)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "0", 
                        $"Entity {entityDescr.EntityType}",
                        entityDescr.ToString().Replace( Environment.NewLine, "|"),
                        "EcsGenerator", 
                        DiagnosticSeverity.Info,
                        true), 
                    declaration.GetLocation())
                );
        }



        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<IFieldSymbol> Fields { get; } = new List<IFieldSymbol>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                Debugger.Break();

                // any field with at least one attribute is a candidate for property generation
                if (context.Node is ClassDeclarationSyntax classDeclarationSyntax
                    && classDeclarationSyntax.AttributeLists.Count > 0)
                {
                    var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                    //classDeclarationSyntax.AttributeLists.
                    //classDeclarationSyntax.AttributeLists.Any(x=>x.At)

                    var ctors = 
                        classDeclarationSyntax.Members.OfType<ConstructorDeclarationSyntax>()
                            .Take(2)
                            .ToArray();
                    if (ctors.Length == 0 || ctors.Length > 1)
                    {
                        return;
                    }
                    var targetCtor = ctors.First();
                    //foreach (VariableDeclaratorSyntax variable in classDeclarationSyntax.Declaration.Variables)
                    //{
                    //    // Get the symbol being declared by the field, and keep it if its annotated
                    //    IFieldSymbol fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
                    //    if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass.ToDisplayString() == "AutoNotify.AutoNotifyAttribute"))
                    //    {
                    //        Fields.Add(fieldSymbol);
                    //    }
                    //}
                }
            }
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
