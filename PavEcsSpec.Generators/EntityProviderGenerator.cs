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
            StringBuilder extraArgumentsDeclaration = new StringBuilder();
            StringBuilder extraArgumentsPass = new StringBuilder();

            StringBuilder initRequiredComponents = new StringBuilder();
            StringBuilder ensureRequiredComponents = new StringBuilder();

            StringBuilder checkRequiredComponentsFormat = new StringBuilder();


            foreach (var baseEntityDescriptor in entityDescr.BaseEntities)
            {
                var providerName = GetProviderName(baseEntityDescriptor);
                var providerFieldName = "_" + providerName;
                var methodDeclaration = $@"
public partial {baseEntityDescriptor.EntityType} {baseEntityDescriptor.Method.Name}()
{{
    return (_provider.{providerFieldName}.TryGetUnsafe(_ent)) ?? throw new ArgumentNullException(""{baseEntityDescriptor.Method.Name}"");
}}";

                var debugProp = $@"
#if DEBUG
public  {baseEntityDescriptor.EntityType} DEBUG_{baseEntityDescriptor.Method.Name}
{{
    get => _provider.{providerFieldName}.TryGetUnsafe(_ent) ?? default;
}}
#endif
";
                accessToDataCode.AppendLine(methodDeclaration);
                accessToDataCode.AppendLine(debugProp);


                providerFieldsCode.AppendLine($"public readonly {baseEntityDescriptor.EntityType}.Provider {providerFieldName};");

                extraArgumentsDeclaration.Append($", {baseEntityDescriptor.EntityType}.Provider {providerName}");
                extraArgumentsPass.Append($", {providerName}");

                initFieldsCode.AppendLine($"{providerFieldName} = {providerName};");

                initRequiredComponents.AppendLine($"{providerFieldName}.Ensure(entId);");
                ensureRequiredComponents.AppendLine($"{providerFieldName}.Ensure(entId);");

                checkRequiredComponentsFormat.AppendLine($"if (!{providerFieldName}.Is(entId)) {{0}}");

            }


            foreach (var componentDescriptor in entityDescr.Components)
            {
                var poolName = GetPoolName(componentDescriptor);
                if (componentDescriptor.ReturnType == null)
                {
                    var readonlyReturn =
                    componentDescriptor.AccessKind == ComponentDescriptorAccessKind.IncludeReadonly ? "readonly" : string.Empty;
                    var methodDeclaration = $@"
public partial ref {readonlyReturn} {componentDescriptor.ComponentType} {componentDescriptor.Method.Name}()
{{
    return ref _provider.{poolName}.Get(_ent);
}}";

                    var property = $@"
#if DEBUG
public {componentDescriptor.ComponentType} Debug_{componentDescriptor.Method.Name}
{{
    get => _provider.{poolName}.Get(_ent);
}}
#endif
";
                    accessToDataCode.AppendLine(methodDeclaration);
                    accessToDataCode.AppendLine(property);

                }
                else
                {
                    //var returnTypeName = $"{componentDescriptor.ReturnName}<{componentDescriptor.TypeName}>";
                    var methodDeclaration = $@"
public partial {componentDescriptor.ReturnType} {componentDescriptor.Method.Name}()
{{
    return new {componentDescriptor.ReturnType}(_provider.{poolName}, _ent);
}}";

                    var property = $@"
#if DEBUG
public {componentDescriptor.ReturnType} Debug_{componentDescriptor.Method.Name}
{{
    get => new {componentDescriptor.ReturnType}(_provider.{poolName}, _ent);
}}
#endif
";
                    accessToDataCode.AppendLine(methodDeclaration);
                    accessToDataCode.AppendLine(property);
                }

                providerFieldsCode.AppendLine($"public readonly Leopotam.EcsLite.EcsPool<{componentDescriptor.ComponentType}> {poolName};");


                initFieldsCode.AppendLine($"{poolName} = world.GetPool<{componentDescriptor.ComponentType}>();");

            }

            static string GetPoolName(in ComponentDescriptor componentDescriptor) => $"_{componentDescriptor.Method.Name.ToLowerInvariant()}Pool";

            static string GetProviderName(in BaseEntityDescriptor baseEntDescriptor) => $"{baseEntDescriptor.Method.Name.ToLowerInvariant()}Provider";


            var name = entityDescr.EntityType.Name;
            StringBuilder providerMethods = new StringBuilder();
            if (!entityDescr.SkipFilter)
            {
                providerFieldsCode.AppendLine("private readonly Leopotam.EcsLite.EcsFilter _filter;");
                initFieldsCode.AppendLine($"_filter = {GetFilterCode(entityDescr)};");
                providerMethods.Append($@"
 [MethodImpl(MethodImplOptions.AggressiveInlining)]
public Enumerator GetEnumerator()
    => new Enumerator(_filter.GetEnumerator(), this);

public Leopotam.EcsLite.EcsFilter Filter => _filter;

public struct Enumerator : IDisposable 
{{
    private Leopotam.EcsLite.EcsFilter.Enumerator _enumerator;
    private readonly Provider _provider;

    public Enumerator(Leopotam.EcsLite.EcsFilter.Enumerator enumerator, Provider provider)
    {{
        _enumerator = enumerator;
        _provider = provider;
    }}

    public {name} Current
    {{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new {name}(_enumerator.Current, _provider);
    }}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {{
        return _enumerator.MoveNext();
    }}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {{
        _enumerator.Dispose();
    }}
}}
");
            }
            string providerCode;


            foreach (var componentDescriptor in entityDescr.Components)
            {
                var poolName = GetPoolName(componentDescriptor);
                if (componentDescriptor.AccessKind.IsRequired())
                {
                    var add = $"{poolName}.Add(entId);";
                    var check = $"if (!{poolName}.Has(entId)) {{0}}";

                    initRequiredComponents.AppendLine(add);
                    checkRequiredComponentsFormat.AppendLine(check);
                    
                    ensureRequiredComponents.AppendLine(String.Format(check, add));
                }
                else if (componentDescriptor.AccessKind == ComponentDescriptorAccessKind.Exclude)
                {
                    checkRequiredComponentsFormat.AppendLine($"if ({poolName}.Has(entId)) {{0}}");
                }
            }


            providerCode = $@"
    public class Provider
    {{
{providerFieldsCode.ToString().PadLeftAllLines(4 * 2)}
        public readonly Leopotam.EcsLite.EcsWorld _world;

        public Provider(Leopotam.EcsLite.EcsWorld world {extraArgumentsDeclaration})
        {{
            _world = world;
{initFieldsCode.ToString().PadLeftAllLines(4 * 3)}
        }}
{providerMethods.ToString().PadLeftAllLines(4 * 2)}

        public {name} New()
        {{
            var entId = _world.NewEntity();
            return Add(entId);
        }}

        public {name} Add(int entId)
        {{
{initRequiredComponents.ToString().PadLeftAllLines(4 * 3)}
            return new {name}(entId, this);
        }}

        public {name} Ensure(int entId)
        {{
{ensureRequiredComponents.ToString().PadLeftAllLines(4 * 3)}
            return new {name}(entId, this);
        }}

        public bool Is(int entId)
        {{
{string.Format(checkRequiredComponentsFormat.ToString(), "return false;").PadLeftAllLines(4 * 3)}
            return true;
        }}

        public {name}? TryGet(Leopotam.EcsLite.EcsPackedEntityWithWorld entity)
        {{
            if (Leopotam.EcsLite.EcsEntityExtensions.Unpack(entity, out var world, out var entId) && world == _world)
            {{
                if (_world != world)
                    throw new System.InvalidOperationException($""Unexpected world: Actual: {{world}}. Expected: {{_world}} "");

                return TryGetUnsafe(entId);
            }}
            return default;
        }}

        public bool TryGet(Leopotam.EcsLite.EcsPackedEntityWithWorld entity, out {name} ent)
        {{
            if (Leopotam.EcsLite.EcsEntityExtensions.Unpack(entity, out var world, out var entId) && world == _world)
            {{
                if (_world != world)
                    throw new System.InvalidOperationException($""Unexpected world: Actual: {{world}}. Expected: {{_world}} "");

                return TryGetUnsafe(entId, out ent);
            }}
            ent = default;
            return false;
        }}

        public {name}? TryGetUnsafe(int entId)
        {{
{string.Format(checkRequiredComponentsFormat.ToString(), "return default;").PadLeftAllLines(4 * 3)}
            return new {name}(entId, this);
        }}

        public bool TryGetUnsafe(int entId, out {name} ent)
        {{
{string.Format(checkRequiredComponentsFormat.ToString(), "{ent = default; return false;}").PadLeftAllLines(4 * 3)}
            ent = new {name}(entId, this);
            return true;
        }}
    }}
";

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

    public int GetRawId() => _ent;

    public Leopotam.EcsLite.EcsPackedEntityWithWorld Id => Leopotam.EcsLite.EcsEntityExtensions.PackEntityWithWorld(_provider._world, _ent);

    public static Provider Create(Leopotam.EcsLite.EcsSystems systems{extraArgumentsDeclaration}) 
    {{
        const string worldName = ""{worldName}"";
        var world = systems.GetWorld(worldName);
        if (world == null)
        {{
            world = new Leopotam.EcsLite.EcsWorld();
            systems.AddWorld(world, worldName);
        }}

        return new Provider(world{extraArgumentsPass});
    }}

    public static Provider Create(Leopotam.EcsLite.EcsWorld world{extraArgumentsDeclaration}) 
    {{
        return new Provider(world{extraArgumentsPass});
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
