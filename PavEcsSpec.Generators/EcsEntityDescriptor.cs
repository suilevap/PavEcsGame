using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PavEcsSpec.Generators
{
    public enum ComponentDescriptorAccessKind
    {
        Include,
        IncludeReadonly,
        Optional,
        Exclude
    }

    public enum EntityKind
    {
        None,
        Filter,
        Factory
    }
    public readonly struct ComponentDescriptor
    {
        public ITypeSymbol ComponentType { get; }

        public ITypeSymbol ReturnType { get; }

        public IMethodSymbol Method { get; }

        public ComponentDescriptorAccessKind AccessKind { get; }

        public ComponentDescriptor(
            ITypeSymbol type,
            IMethodSymbol method,
            ComponentDescriptorAccessKind accessType,
            ITypeSymbol returnType = null)
        {
            ComponentType = type;
            Method = method;
            AccessKind = accessType;
            ReturnType = returnType;
        }
        public override string ToString()
        {
            return $"{ReturnType} {ComponentType} {AccessKind} {Method}";
        }
    }
    internal class EcsEntityDescriptor
    {
        private readonly List<ComponentDescriptor> _components = new List<ComponentDescriptor>();
        public ITypeSymbol EntityType { get; private set; }
        public IEnumerable<ComponentDescriptor> Components => _components;

        public string Universe { get; private set; }
        public IMethodSymbol GetIdMethod { get; private set; }
        public IMethodSymbol ProviderMethod { get; private set; }
        public EntityKind Kind { get; private set; }

        //public IMethodSymbol FactoryMethod { get; private set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"{EntityType}/{Universe} ");
            foreach (var component in _components)
            {
                result.AppendLine(component.ToString());
            }
            result.AppendLine($"Provider {ProviderMethod?.Name}, Id: {GetIdMethod?.Name} ");

            return result.ToString();
        }


        public static EcsEntityDescriptor Create(ITypeSymbol type)
        {
            EcsEntityDescriptor result = new EcsEntityDescriptor();
            result.EntityType = type;
            ParseAttributes(type, result);

            foreach (ISymbol member in type.GetMembers())
            {
                if (member is IMethodSymbol methodSymbol &&
                    methodSymbol.IsPartialDefinition)
                {
                    var returnType = methodSymbol.ReturnType;
                    if (!methodSymbol.IsStatic)
                    {
                        if (returnType.IsValueType)
                        {
                            if (methodSymbol.ReturnsByRefReadonly) //include readonly
                            {
                                result._components.Add(
                                    new ComponentDescriptor(returnType,
                                    methodSymbol,
                                        ComponentDescriptorAccessKind.IncludeReadonly));
                            }
                            else if (methodSymbol.ReturnsByRef) //include
                            {
                                result._components.Add(
                                    new ComponentDescriptor(
                                        returnType,
                                        methodSymbol,
                                        ComponentDescriptorAccessKind.Include));
                            }
                            else if (returnType.SpecialType == SpecialType.System_Int32)
                            {
                                result.GetIdMethod = methodSymbol;
                            }
                        }
                        if (returnType.IsRefLikeType && returnType is INamedTypeSymbol namedType &&
                            namedType.Arity == 1)
                        {
                            var componentType = namedType.TypeArguments.First();

                            if (componentType.IsValueType)
                            {
                                if (namedType.Name == "OptionalComponent")
                                {
                                    result._components.Add(
                                        new ComponentDescriptor(
                                            componentType,
                                            methodSymbol,
                                            ComponentDescriptorAccessKind.Optional,
                                            namedType));
                                }
                                else if (namedType.Name == "ExcludeComponent")
                                {
                                    result._components.Add(
                                        new ComponentDescriptor(
                                            componentType,
                                            methodSymbol,
                                            ComponentDescriptorAccessKind.Exclude,
                                            namedType));
                                }
                                else
                                {
                                    throw new InvalidOperationException($"unexpected return type {namedType}");
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException($"unexpected non value return type {componentType}");
                            }
                        }
                    }
                    else //static method
                    {
                        if (returnType is INamedTypeSymbol namedType &&
                            namedType.Arity == 1)
                        {
                            var generictType = namedType.TypeArguments.First();
                            if (!SymbolEqualityComparer.Default.Equals(generictType, type))
                                throw new InvalidOperationException($"unexpected return type {generictType}");

                            if (namedType.Name == "IEntityProvider")
                            {
                                result.Kind = EntityKind.Filter;
                                result.ProviderMethod= methodSymbol;
                            }
                            else if (namedType.Name == "IEntityFactory")
                            {
                                result.Kind = EntityKind.Factory;
                                result.ProviderMethod = methodSymbol;
                            }
                            else
                            {
                                throw new InvalidOperationException($"unexpected return type {namedType}");
                            }

                        }
                    }
                    //else if (methodSymbol.ReturnType.) 
                }
            }

            return result;
        }

        private static void ParseAttributes(ITypeSymbol type, EcsEntityDescriptor result)
        {
            var entityAttribute = type.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass.Name == nameof(EcsInfraTypes.EntityAttribute));

            if (entityAttribute != null)
            {
                var universeProp = entityAttribute
                    .NamedArguments
                    .FirstOrDefault(x => x.Key == "Universe");
                result.Universe = universeProp.Value.Value?.ToString() ?? String.Empty;
            }
        }
    }

}
