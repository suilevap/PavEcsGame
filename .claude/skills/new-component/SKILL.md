---
name: new-component
description: Create a new ECS component struct (data, tag, or event) with proper conventions and cleanup registration
argument-hint: [ComponentName] [type: data|tag|event]
---

# Create a New ECS Component

Create a new component for the PavEcsGame roguelike. Arguments: `$ARGUMENTS`
- `$0` = Component name (e.g., `Health`, `Poison`, `Teleport`)
- `$1` = Component type: `data`, `tag`, or `event`. Default: `data`

## Step 1: Determine the component type

### Data component
A struct with fields that holds entity state. Examples: `SpeedComponent`, `PositionComponent`, `LightSourceComponent`.

### Tag component
An empty marker struct implementing `IEcsIgnoreInFilter`. Used to flag entities for processing. Examples: `IsActiveTag`, `DestroyRequestTag`, `MarkAsRenderedTag`.

### Event component
A temporary struct that exists for one frame and is automatically cleaned up via `MyDelHere<T>()`. Used for inter-system communication. Examples: `CollisionEvent<T>`, `MapLoadedEvent`.

## Step 2: Choose the file location

- **Data components** → `PavEcsGame.Components/Components/$0Component.cs`
- **Tag components** → `PavEcsGame.Components/Components/Tags/$0Tag.cs`
- **Event components** → `PavEcsGame.Components/Components/Events/$0Event.cs`
- **System-internal components** → `PavEcsGame.Components/Components/SystemComponents/$0Component.cs`

## Step 3: Create the component file

### Template A: Data component

```csharp
using System.Diagnostics;

namespace PavEcsGame.Components
{
    [DebuggerDisplay("$0: {Value}")]
    public struct $0Component
    {
        // TODO: Add fields
        public int Value;
    }
}
```

If the component needs equality comparison (used in change detection or deduplication), implement `IEquatable<T>`:

```csharp
using System;
using System.Runtime.CompilerServices;

namespace PavEcsGame.Components
{
    public struct $0Component : IEquatable<$0Component>
    {
        public int Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals($0Component other) => Value == other.Value;

        public override bool Equals(object obj) => obj is $0Component other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in $0Component lhs, in $0Component rhs) => lhs.Value == rhs.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in $0Component lhs, in $0Component rhs) => lhs.Value != rhs.Value;
    }
}
```

Use `[MethodImpl(MethodImplOptions.AggressiveInlining)]` on operators and Equals for performance-critical components.

### Template B: Tag component

```csharp
namespace PavEcsGame.Components
{
    public struct $0Tag : IEcsIgnoreInFilter
    {
    }
}
```

`IEcsIgnoreInFilter` (defined in `PavEcsGame.Components/Interfaces/FakeEcsInterfaces.cs`) extends `ITag`. Tags have no fields — they are pure markers.

### Template C: Event component

```csharp
namespace PavEcsGame.Components.Events
{
    public struct $0Event
    {
        // TODO: Add event data fields
    }
}
```

For generic event components (parameterized by entity type):
```csharp
namespace PavEcsGame.Components
{
    public struct $0Event<T> where T : struct, IEntity
    {
        public T Source;
        public T Target;
    }
}
```

## Step 4: Register cleanup (event components only)

For event components, add `MyDelHere` in `PavEcsLiteGame/GameLoop/GameMainContainer.cs` in the cleanup section (before `.Init()`):

```csharp
.MyDelHere<$0Event>()
```

For generic event components:
```csharp
.MyDelHere<$0Event<EcsEntity>>()
```

This ensures the event component is automatically removed from all entities every frame.

## Step 5: Use in systems

Reference the new component in system `[Entity]` structs:

```csharp
// Data component — read-only access (included in filter):
public partial ref readonly $0Component Value();

// Data component — read-write access (included in filter):
public partial ref $0Component Value();

// Optional access (not in filter, may or may not exist):
public partial OptionalComponent<$0Component> Value();

// Tag component — read-only check (included in filter):
public partial ref readonly $0Tag Tagged();

// Tag component — optional tagging:
public partial RequiredComponent<$0Tag> Tagged();
// Usage: ent.Tagged().TryTag(true);  // add tag
//        ent.Tagged().TryTag(false); // remove tag

// Event component — write (to create events):
public partial OptionalComponent<$0Event> Event();
// Usage: ent.Event().Ensure() = new $0Event { ... };
```

## Checklist

- [ ] Component file created in correct directory with correct namespace
- [ ] Data components: fields defined, `[DebuggerDisplay]` added
- [ ] Tag components: implements `IEcsIgnoreInFilter`, no fields
- [ ] Event components: `MyDelHere<T>()` registered in `GameMainContainer.cs`
- [ ] Solution builds: `dotnet build PavEcsGame.sln`
