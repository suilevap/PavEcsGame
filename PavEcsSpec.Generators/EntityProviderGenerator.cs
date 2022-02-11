using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PavEcsSpec.Generators
{
    internal class EntityProviderGenerator
    {
        private enum ComponentDescriptorType
        {
            Include,
            IncludeReadonly,
            Optional,
            Exclude
        }
        private readonly struct ComponentDescriptor
        {
            public string TypeName { get; }

            public string ReturnName { get; }

            public string MethodName { get; }

            public ComponentDescriptorType Type { get; }

            public ComponentDescriptor(string typeName, string methodName, ComponentDescriptorType type, string returnName = null)
            {
                TypeName = typeName;
                MethodName = methodName;
                Type = type;
                ReturnName = returnName;
            }
        }

        private class EntityDescriptor
        {
            public string Name { get; set; }
            public List<ComponentDescriptor> Components { get; } = new List<ComponentDescriptor>();
        }

        private EntityDescriptor GetEntityDescriptor(ITypeSymbol type)
        {
            EntityDescriptor result = new EntityDescriptor();
            result.Name = type.Name;
            foreach (ISymbol member in type.GetMembers())
            {
                //if (member.Kind == SymbolKind.Method)
                //{
                //    member.Is
                //}

                if (member is IMethodSymbol methodSymbol && methodSymbol.IsPartialDefinition)
                {
                    var returnType = methodSymbol.ReturnType;
                    if (returnType.IsValueType)
                    {
                        var name = $"{returnType.ContainingNamespace}.{returnType.Name}";
                        if (methodSymbol.ReturnsByRefReadonly) //include readonly
                        {
                            
                            result.Components.Add(
                                new ComponentDescriptor(name, methodSymbol.Name,
                                    ComponentDescriptorType.IncludeReadonly));
                        }
                        else if (methodSymbol.ReturnsByRef) //include
                        {
                            result.Components.Add(
                                new ComponentDescriptor(name, methodSymbol.Name,
                                    ComponentDescriptorType.Include));
                        }
                    }

                    if (returnType.IsRefLikeType && returnType is INamedTypeSymbol namedType && namedType.Arity == 1)
                    {
                        var componentType = namedType.TypeArguments.First();
                        var name = $"{componentType.ContainingNamespace}.{componentType.Name}";

                        if (componentType.IsValueType)
                        {
                            var namedTypeFullName = $"{namedType.ContainingNamespace}.{namedType.Name}";

                            if (namedType.Name.Contains("Optional"))
                            {
                                result.Components.Add(
                                    new ComponentDescriptor(name, methodSymbol.Name,
                                        ComponentDescriptorType.Optional, namedTypeFullName));
                            }
                            else if (namedType.Name.Contains("Exclude"))
                            {
                                result.Components.Add(
                                    new ComponentDescriptor(name, methodSymbol.Name,
                                        ComponentDescriptorType.Exclude, namedTypeFullName));
                            }
                        }
                    }
                    //else if (methodSymbol.ReturnType.) 
                }
            }

            return result;
        }

        public string GenerateEntityCode(ITypeSymbol type, StructDeclarationSyntax syntax)
        {
            var ns = type.ContainingNamespace.ToString();
            var name = type.Name;
            var entityDescr = GetEntityDescriptor(type);

            StringBuilder accessToDataCode = new StringBuilder();
            StringBuilder providerFieldsCode = new StringBuilder();
            StringBuilder initFieldsCode = new StringBuilder();

            foreach (var componentDescriptor in entityDescr.Components)
            {
                var poolName = GetPoolName(componentDescriptor);
                if (componentDescriptor.Type == ComponentDescriptorType.Include
                    || componentDescriptor.Type == ComponentDescriptorType.IncludeReadonly)
                {
                    var readonlyReturn =
                    componentDescriptor.Type == ComponentDescriptorType.IncludeReadonly ? "readonly" : string.Empty;
                    var methodDeclaration = $@"
public partial ref {readonlyReturn} {componentDescriptor.TypeName} {componentDescriptor.MethodName}()
{{
    return ref _provider.{poolName}.Get(_ent);
}}";
                    accessToDataCode.AppendLine(methodDeclaration);
                }
                if (componentDescriptor.Type == ComponentDescriptorType.Optional
                    || componentDescriptor.Type == ComponentDescriptorType.Exclude)
                {
                    var returnTypeName = $"{componentDescriptor.ReturnName}<{componentDescriptor.TypeName}>";
                    var methodDeclaration = $@"
public partial {returnTypeName} {componentDescriptor.MethodName}()
{{
    return new {returnTypeName}(_provider.{poolName}, _ent);
}}";
                    accessToDataCode.AppendLine(methodDeclaration);
                }

                providerFieldsCode.AppendLine($"public readonly Leopotam.EcsLite.EcsPool<{componentDescriptor.TypeName}> {poolName};");

                initFieldsCode.AppendLine($"{poolName} = world.GetPool<{componentDescriptor.TypeName}>();");

            }
            var includes = entityDescr.Components.Where(x => x.Type == ComponentDescriptorType.Include
                                              || x.Type == ComponentDescriptorType.IncludeReadonly)
                .ToArray();
            var excludes = entityDescr.Components.Where(x => x.Type == ComponentDescriptorType.Exclude)
                .ToArray();
            if (!includes.Any())
            {
                throw new InvalidOperationException($"Entity {name} should have at leas one required component");
            }
            StringBuilder filterCode = new StringBuilder();

            filterCode.Append($"world.Filter<{includes.First().TypeName}>()");
            foreach (var componentDescriptor in includes.Skip(1))
            {
                filterCode.Append($".Inc<{componentDescriptor.TypeName}>()");
            }
            foreach (var componentDescriptor in excludes)
            {
                filterCode.Append($".Exc<{componentDescriptor.TypeName}>()");
            }
            filterCode.Append($".End()");

            string GetPoolName(in ComponentDescriptor componentDescriptor) => $"_{componentDescriptor.MethodName.ToLowerInvariant()}Pool";

            var result = @$"
private readonly partial struct {name}
{{
    private readonly int _ent;

    private readonly Provider _provider;

    private {name}(int ent, Provider provider)
    {{
        _ent = ent;
        _provider = provider;
    }}

{accessToDataCode.ToString().PadLeftAllLines(4*1)}

    public static partial PavEcsSpec.Generated.IEntityProvider<{name}> GetProvider(Leopotam.EcsLite.EcsSystems systems) 
        => new Provider(systems.GetWorld());

    private class Provider : PavEcsSpec.Generated.IEntityProvider<{name}>
    {{
{providerFieldsCode.ToString().PadLeftAllLines(4*2)}
        private readonly Leopotam.EcsLite.EcsFilter _filter;

        public Provider(Leopotam.EcsLite.EcsWorld world)
        {{
{initFieldsCode.ToString().PadLeftAllLines(4*3)}
            _filter = {filterCode.ToString()};
        }}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public {name} Get(int ent) => new {name}(ent, this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PavEcsSpec.Generated.BaseEnumerator<{name}> GetEnumerator()
            => new PavEcsSpec.Generated.BaseEnumerator<{name}>(_filter.GetEnumerator(), this);
       
    }}
}}";

            return result;
        //    var a = @$" public struct Enumerator : IDisposable
        //{{
        //    private EcsFilter.Enumerator _enumerator;
        //    private readonly Provider _provider;

        //    public Enumerator(EcsFilter.Enumerator enumerator, Provider provider)
        //    {{
        //        _enumerator = enumerator;
        //        _provider = provider;
        //    }}

        //    public {name} Current
        //    {{
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        get => new {name}(_enumerator.Current, _provider);
        //    }}

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    public bool MoveNext()
        //    {{
        //        return _enumerator.MoveNext();
        //    }}

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    public void Dispose()
        //    {{
        //        _enumerator.Dispose();
        //    }}
        //}}";
        }
    }
}
