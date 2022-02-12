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

        public string GenerateEntityCode(EcsEntityDescriptor entityDescr, int wolrdId)
        {

            StringBuilder accessToDataCode = new StringBuilder();
            StringBuilder providerFieldsCode = new StringBuilder();
            StringBuilder initFieldsCode = new StringBuilder();

            foreach (var componentDescriptor in entityDescr.Components)
            {
                var poolName = GetPoolName(componentDescriptor);
                if (componentDescriptor.AccessType == ComponentDescriptorType.Include
                    || componentDescriptor.AccessType == ComponentDescriptorType.IncludeReadonly)
                {
                    var readonlyReturn =
                    componentDescriptor.AccessType == ComponentDescriptorType.IncludeReadonly ? "readonly" : string.Empty;
                    var methodDeclaration = $@"
public partial ref {readonlyReturn} {componentDescriptor.ComponentType} {componentDescriptor.Method.Name}()
{{
    return ref _provider.{poolName}.Get(_ent);
}}";
                    accessToDataCode.AppendLine(methodDeclaration);
                }
                if (componentDescriptor.AccessType == ComponentDescriptorType.Optional
                    || componentDescriptor.AccessType == ComponentDescriptorType.Exclude)
                {
                    //var returnTypeName = $"{componentDescriptor.ReturnName}<{componentDescriptor.TypeName}>";
                    var methodDeclaration = $@"
public partial {componentDescriptor.ReturnType} {componentDescriptor.Method.Name}()
{{
    return new {componentDescriptor.ReturnType}(_provider.{poolName}, _ent);
}}";
                    accessToDataCode.AppendLine(methodDeclaration);
                }

                providerFieldsCode.AppendLine($"public readonly Leopotam.EcsLite.EcsPool<{componentDescriptor.ComponentType}> {poolName};");

                initFieldsCode.AppendLine($"{poolName} = world.GetPool<{componentDescriptor.ComponentType}>();");

            }
            var includes = entityDescr.Components.Where(x => x.AccessType == ComponentDescriptorType.Include
                                              || x.AccessType == ComponentDescriptorType.IncludeReadonly)
                .ToArray();
            var excludes = entityDescr.Components.Where(x => x.AccessType == ComponentDescriptorType.Exclude)
                .ToArray();
            if (!includes.Any())
            {
                throw new InvalidOperationException($"Entity {entityDescr.EntityType} should have at leas one required component");
            }
            StringBuilder filterCode = new StringBuilder();

            filterCode.Append($"world.Filter<{includes.First().ComponentType}>()");
            foreach (var componentDescriptor in includes.Skip(1))
            {
                filterCode.Append($".Inc<{componentDescriptor.ComponentType}>()");
            }
            foreach (var componentDescriptor in excludes)
            {
                filterCode.Append($".Exc<{componentDescriptor.ComponentType}>()");
            }
            filterCode.Append($".End()");

            string GetPoolName(in ComponentDescriptor componentDescriptor) => $"_{componentDescriptor.Method.Name.ToLowerInvariant()}Pool";

            var name = entityDescr.EntityType.Name;
            var worldName = $"GENERATED_u{entityDescr.Universe}w{wolrdId}";
            var result = @$"
/* 
{entityDescr.ToString().Replace("|",Environment.NewLine)}
*/
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
        => new Provider(systems.GetWorld({worldName}));

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
        }
    }
}
