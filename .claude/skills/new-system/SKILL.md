---
name: new-system
description: Create a new ECS system with proper boilerplate, entity queries, TurnManager integration, and GameMainContainer registration
argument-hint: [SystemName] [type: sim|tick|simple]
---

# Create a New ECS System

Create a new system for the PavEcsGame roguelike. Arguments: `$ARGUMENTS`
- `$0` = System name (e.g., `Poison`, `Teleport`) — will produce `$0System.cs`
- `$1` = System type: `sim` (simulation-registered), `tick` (TickUpdate-gated), or `simple` (no TurnManager). Default: `sim`

## Step 1: Determine the system type

Ask the user if not clear from arguments. The three patterns are:

### Type A: Simulation system (`sim`)
Runs during Simulation phase. Registers with TurnManager and reports whether it still has work to do. Used for: physics, movement resolution, destruction, position updates.

### Type B: TickUpdate-gated system (`tick`)
Only runs during TickUpdate phase (when no simulation work is pending). Used for: input handling, AI decisions, command creation.

### Type C: Simple system (`simple`)
Runs every frame regardless of phase. No TurnManager dependency. Used for: rendering, tile updates, visual effects.

## Step 2: Create the system file

Create `PavEcsLiteGame/Systems/$0System.cs` (or an appropriate subdirectory like `Controls/`, `Renders/`, `Managers/`, `Utils/`).

### Template A: Simulation system

```csharp
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class $0System : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly Providers _providers;

        private readonly TurnManager _turnManager;
        private TurnManager.SimSystemRegistration _reg;

        [Entity]
        private readonly partial struct TargetEnt
        {
            // TODO: Define component queries
            // public partial ref readonly PositionComponent Pos();   // read-only, included in filter
            // public partial ref SpeedComponent Speed();             // read-write, included in filter
            // public partial OptionalComponent<NewPositionComponent> NewPos();  // optional, not in filter
        }

        public $0System(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }

        public void Init(IEcsSystems systems)
        {
            _reg = _turnManager.RegisterSimulationSystem(this);
        }

        public void Run(IEcsSystems systems)
        {
            var hasWorkToDo = false;
            foreach (var entity in _providers.TargetEntProvider)
            {
                hasWorkToDo = true;
                // TODO: System logic
            }

            _reg.UpdateState(hasWorkToDo);
        }
    }
}
```

### Template B: TickUpdate-gated system

```csharp
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class $0System : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly Providers _providers;

        private readonly TurnManager _turnManager;

        [Entity]
        private readonly partial struct TargetEnt
        {
            // TODO: Define component queries
            // public partial ref readonly IsActiveTag Active();
            // public partial OptionalComponent<MoveCommandComponent> Move();
        }

        public $0System(TurnManager turnManager, EcsSystems universe)
            : this(universe)
        {
            _turnManager = turnManager;
        }

        public void Run(IEcsSystems systems)
        {
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;

            foreach (var ent in _providers.TargetEntProvider)
            {
                // TODO: System logic (create commands, make decisions)
            }
        }
    }
}
```

### Template C: Simple system

```csharp
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class $0System : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly Providers _providers;

        [Entity]
        private readonly partial struct TargetEnt
        {
            // TODO: Define component queries
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in _providers.TargetEntProvider)
            {
                // TODO: System logic
            }
        }
    }
}
```

If the system also needs one-time initialization, add `IEcsInitSystem` to the interface list and implement:
```csharp
public void Init(IEcsSystems systems)
{
    // One-time setup
}
```

## Step 3: Register in GameMainContainer

In `PavEcsLiteGame/GameLoop/GameMainContainer.cs`, add the system in the correct section of the `.Add()` chain:

- **Simulation systems** go after existing simulation systems (MovementSystem, FrictionSystem, etc.) and before rendering
- **TickUpdate-gated systems** go in the input/command section (after CommandTokenDistributionSystem, before MoveCommandSystem)
- **Simple systems** go wherever appropriate for their purpose (rendering section, cleanup section, etc.)

```csharp
// For sim or tick systems with TurnManager:
.Add(new $0System(turnManager, _systems))

// For simple systems without TurnManager:
.Add(new $0System(_systems))
```

## Entity struct reference

Return types on entity methods control filter behavior and access:

| Return type | Filter | Access | Example use |
|---|---|---|---|
| `ref readonly T` | Include | Read-only | Reading position, checking tags |
| `ref T` | Include | Read-write | Modifying speed, updating state |
| `OptionalComponent<T>` | Not in filter | Read/write/create/remove | Adding commands, conditional data |
| `RequiredComponent<T>` | Include (via wrapper) | Read/write/remove | When you need `.Has()` or `.Remove()` |
| `ExcludeComponent<T>` | Exclude | Create only | Filtering out entities with a component |
| `T` (value return) | Include | Read-only copy | Nested entity composition |

Compose entities by returning other entity types:
```csharp
public partial ActorEntity Actor();  // Pulls in all ActorEntity components
```

## Checklist

- [ ] System file created with correct partial class, interfaces, and namespace
- [ ] `[Entity]` struct(s) defined with appropriate component access patterns
- [ ] Constructor calls `: this(universe)` to invoke generated constructor
- [ ] Simulation systems: `_reg = _turnManager.RegisterSimulationSystem(this)` in Init, `_reg.UpdateState()` in Run
- [ ] TickUpdate systems: phase gate `if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate) return;`
- [ ] System registered in `GameMainContainer.cs` in correct execution order
- [ ] Solution builds: `dotnet build PavEcsGame.sln`
