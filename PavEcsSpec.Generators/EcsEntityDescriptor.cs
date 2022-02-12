using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PavEcsSpec.Generators
{
    public enum ComponentDescriptorType
    {
        Include,
        IncludeReadonly,
        Optional,
        Exclude
    }
    public readonly struct ComponentDescriptor
    {
        public ITypeSymbol ComponentType { get; }

        public ITypeSymbol ReturnType { get; }

        public IMethodSymbol Method { get; }

        public ComponentDescriptorType AccessType { get; }

        public ComponentDescriptor(
            ITypeSymbol type,
            IMethodSymbol method,
            ComponentDescriptorType accessType,
            ITypeSymbol returnType = null)
        {
            ComponentType = type;
            Method = method;
            AccessType = accessType;
            ReturnType = returnType;
        }
        public override string ToString()
        {
            return $"{ReturnType} {ComponentType} {AccessType} {Method}";
        }
    }
    internal class EcsEntityDescriptor
    {
        private readonly List<ComponentDescriptor> _components = new List<ComponentDescriptor>();
        public ITypeSymbol EntityType { get; private set; }
        public IEnumerable<ComponentDescriptor> Components => _components;

        public string Universe { get; private set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"{EntityType}/{Universe} ");
            foreach (var component in _components)
            {
                result.AppendLine(component.ToString());
            }

            return result.ToString();
        }


        public static EcsEntityDescriptor Create(ITypeSymbol type)
        {
            EcsEntityDescriptor result = new EcsEntityDescriptor();
            result.EntityType = type;
            ParseAttributes(type, result);

            foreach (ISymbol member in type.GetMembers())
            {

                if (member is IMethodSymbol methodSymbol && methodSymbol.IsPartialDefinition)
                {
                    var returnType = methodSymbol.ReturnType;
                    if (returnType.IsValueType)
                    {
                        if (methodSymbol.ReturnsByRefReadonly) //include readonly
                        {

                            result._components.Add(
                                new ComponentDescriptor(returnType,
                                methodSymbol,
                                    ComponentDescriptorType.IncludeReadonly));
                        }
                        else if (methodSymbol.ReturnsByRef) //include
                        {
                            result._components.Add(
                                new ComponentDescriptor(
                                    returnType,
                                    methodSymbol,
                                    ComponentDescriptorType.Include));
                        }
                    }

                    if (returnType.IsRefLikeType && returnType is INamedTypeSymbol namedType && namedType.Arity == 1)
                    {
                        var componentType = namedType.TypeArguments.First();

                        if (componentType.IsValueType)
                        {
                            if (namedType.Name.Contains("Optional"))
                            {
                                result._components.Add(
                                    new ComponentDescriptor(
                                        componentType,
                                        methodSymbol,
                                        ComponentDescriptorType.Optional,
                                        namedType));
                            }
                            else if (namedType.Name.Contains("Exclude"))
                            {
                                result._components.Add(
                                    new ComponentDescriptor(
                                        componentType,
                                        methodSymbol,
                                        ComponentDescriptorType.Exclude,
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
