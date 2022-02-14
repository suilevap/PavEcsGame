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

        public string GenerateEntityCode(EcsEntityDescriptor entityDescr, string worldName)
        {

            StringBuilder accessToDataCode = new StringBuilder();
            StringBuilder providerFieldsCode = new StringBuilder();
            StringBuilder initFieldsCode = new StringBuilder();

            foreach (var componentDescriptor in entityDescr.Components)
            {
                var poolName = GetPoolName(componentDescriptor);
                if (componentDescriptor.AccessKind.IsRequired())
                {
                    var readonlyReturn =
                    componentDescriptor.AccessKind == ComponentDescriptorAccessKind.IncludeReadonly ? "readonly" : string.Empty;
                    var methodDeclaration = $@"
public partial ref {readonlyReturn} {componentDescriptor.ComponentType} {componentDescriptor.Method.Name}()
{{
    return ref _provider.{poolName}.Get(_ent);
}}";
                    accessToDataCode.AppendLine(methodDeclaration);
                }
                else
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
            if (entityDescr.GetIdMethod != null)
            {
                var methodDeclaration = $@"
public partial {entityDescr.GetIdMethod.ReturnType} {entityDescr.GetIdMethod.Name}()
{{
    return _ent;
}}";
                accessToDataCode.AppendLine(methodDeclaration);
            }

            string GetPoolName(in ComponentDescriptor componentDescriptor) => $"_{componentDescriptor.Method.Name.ToLowerInvariant()}Pool";

            var name = entityDescr.EntityType.Name;
            string providerCode;
            if (entityDescr.Kind == EntityKind.Filter)
            {
                providerCode = $@"
    private class Provider : {entityDescr.ProviderMethod.ReturnType}
    {{
{providerFieldsCode.ToString().PadLeftAllLines(4 * 2)}
        private readonly Leopotam.EcsLite.EcsFilter _filter;

        public Provider(Leopotam.EcsLite.EcsWorld world)
        {{
{initFieldsCode.ToString().PadLeftAllLines(4 * 3)}
            _filter = {GetFilterCode(entityDescr)};
        }}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public {name} Get(int ent) => new {name}(ent, this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PavEcsSpec.Generated.BaseEnumerator<{name}> GetEnumerator()
            => new PavEcsSpec.Generated.BaseEnumerator<{name}>(_filter.GetEnumerator(), this);
       
    }}
";
            }
            else if (entityDescr.Kind == EntityKind.Factory)
            {
                StringBuilder initRequiredComponents = new StringBuilder();
                StringBuilder checkRequiredComponents = new StringBuilder();

                foreach (var componentDescriptor in entityDescr.Components)
                {
                    if (componentDescriptor.AccessKind.IsRequired())
                    {
                        initRequiredComponents.AppendLine($"{GetPoolName(componentDescriptor)}.Add(entId);");
                        checkRequiredComponents.AppendLine($"if (!{GetPoolName(componentDescriptor)}.Has(entId)) return default;");
                    }
                }
                providerCode = $@"
    private class Provider : {entityDescr.ProviderMethod.ReturnType}
    {{
{providerFieldsCode.ToString().PadLeftAllLines(4 * 2)}
        private readonly Leopotam.EcsLite.EcsWorld _world;

        public Provider(Leopotam.EcsLite.EcsWorld world)
        {{
            _world = world;
{initFieldsCode.ToString().PadLeftAllLines(4 * 3)}
        }}

        public {name} New()
        {{
            var entId = _world.NewEntity();
{initRequiredComponents.ToString().PadLeftAllLines(4 * 3)}
            return new {name}(entId, this);
        }}

        public {name}? TryGet(Leopotam.EcsLite.EcsPackedEntityWithWorld entity)
        {{
            if (Leopotam.EcsLite.EcsEntityExtensions.Unpack(entity, out var world, out var entId) && world == _world)
            {{
                if (_world != world)
                    throw new System.InvalidOperationException($""Unexpected world: Actual: {{world}}. Expected: {{_world}} "");

{checkRequiredComponents.ToString().PadLeftAllLines(4 * 4)}

                return new {name}(entId, this);
            }}
            return default;
        }}
    }}
";
            }
            else
            {
                providerCode = string.Empty;
            }
            var result = @$"
/* 
{entityDescr.ToString()}
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

{accessToDataCode.ToString().PadLeftAllLines(4 * 1)}

    public static partial {entityDescr.ProviderMethod.ReturnType} {entityDescr.ProviderMethod.Name}(Leopotam.EcsLite.EcsSystems systems) 
    {{
        const string worldName = ""{worldName}"";
        var world = systems.GetWorld(worldName);
        if (world == null)
        {{
            world = new Leopotam.EcsLite.EcsWorld();
            systems.AddWorld(world, worldName);
        }}

        return new Provider(world);
    }}

    {providerCode}
}}";

            return result;
        }

        private static string GetFilterCode(EcsEntityDescriptor entityDescr)
        {
            var includes = entityDescr.Components.Where(x => x.AccessKind == ComponentDescriptorAccessKind.Include
                                              || x.AccessKind == ComponentDescriptorAccessKind.IncludeReadonly)
                .ToArray();
            var excludes = entityDescr.Components.Where(x => x.AccessKind == ComponentDescriptorAccessKind.Exclude)
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
            return filterCode.ToString();
        }
    }
}
