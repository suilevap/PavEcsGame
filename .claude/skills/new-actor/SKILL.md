---
name: new-actor
description: Create a new actor entity type with turn-based movement, control system, and spawn integration
argument-hint: [ActorName] [symbol] [color]
---

# Create a New Actor

Create a new turn-based actor for the PavEcsGame roguelike. Arguments: `$ARGUMENTS`
- `$0` = Actor name (e.g., `Guard`, `Goblin`)
- `$1` = Display symbol (e.g., `G`, default: first letter of name)
- `$2` = ConsoleColor (e.g., `Magenta`, default: `Yellow`)

## Steps

### 1. Add EntityType to SpawnRequestComponent

In `PavEcsGame.Components/Components/SpawnRequestComponent.cs`, add a new value to the `EntityType` enum:
```csharp
$0,
```

### 2. Define the Entity struct in SpawnEntitySystem

In `PavEcsLiteGame/Systems/SpawnEntitySystem.cs`, add a new entity struct inside the class. Compose from existing base entities as needed:

```csharp
[Entity(SkipFilter = true)]
private readonly partial struct $0Entity
{
    public partial ActorEntity Actor();       // Gives: Position, Speed, IsActive, Collider, VisualSensor
    public partial DirTileEntity DirTile();   // Gives: Direction, DirectionalTile
    // Add any custom components below:
    public partial ref RandomGeneratorComponent Rnd();
    public partial ref SpeedComponent Speed();
    public partial ref SymbolComponent View();
    public partial ref MoveFrictionComponent Friction();
    public partial ref WaitCommandTokenComponent WaitCommandToken();
}
```

Base entity composition reference:
- `CommonEntity` = Position + Speed + IsActiveTag
- `PhysicEntity` = CommonEntity + Collider
- `ActorEntity` = PhysicEntity + VisualSensor
- `DirTileEntity` = CommonEntity + Direction + DirectionalTile
- `LightEntity` = CommonEntity + LightSource

### 3. Handle spawn in TrySpawnEntity()

In the same `SpawnEntitySystem.cs`, add a case in `TrySpawnEntity()`:

```csharp
case EntityType.$0:
    var $0Lower = _providers.$0EntityProvider.Add(ent);
    $0Lower.View() = new SymbolComponent
    {
        Value = '$1',              // Display character
        Depth = Depth.Foreground,
        MainColor = ConsoleColor.$2
    };
    $0Lower.Friction() = new MoveFrictionComponent { FrictionValue = 1 };
    $0Lower.WaitCommandToken() = new WaitCommandTokenComponent(1);  // 1 action per recharge
    $0Lower.Rnd() = new RandomGeneratorComponent { Rnd = rnd };
    result = true;
    break;
```

### 4. Create the control system

Create a new file `PavEcsLiteGame/Systems/Controls/$0ControlSystem.cs`:

```csharp
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.SystemComponents;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems.Controls
{
    internal partial class $0ControlSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly EcsUniverse _universe;
        private readonly TurnManager _turnManager;
        private readonly Providers _providers;

        [Entity]
        private readonly partial struct ControlledEnt
        {
            // Filter: entities with RandomGenerator + IsActive + CommandToken (has actions available)
            public partial ref readonly RandomGeneratorComponent Rnd();
            public partial ref readonly IsActiveTag Active();
            public partial ref readonly CommandTokenComponent Token();
            // Write: create move command
            public partial OptionalComponent<MoveCommandComponent> MoveCommand();
        }

        public $0ControlSystem(TurnManager turnManager, EcsUniverse universe)
        {
            _turnManager = turnManager;
            _universe = universe;
        }

        public void Init(IEcsSystems systems)
        {
            _universe.Register(this);
        }

        public void Run(IEcsSystems systems)
        {
            // Only make decisions during TickUpdate phase
            if (_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate)
                return;

            foreach (var ent in _providers.ControlledEntProvider)
            {
                // TODO: Replace with actual AI behavior
                // Example: random movement (same as RandomMoveSystem)
                var rnd = ent.Rnd().Rnd;
                var moveIndex = rnd.Next(5);
                var move = moveIndex switch
                {
                    1 => new Int2(1, 0),
                    2 => new Int2(-1, 0),
                    3 => new Int2(0, 1),
                    4 => new Int2(0, -1),
                    _ => Int2.Zero
                };

                ent.MoveCommand().Ensure() = new MoveCommandComponent
                {
                    Target = new PositionComponent(move),
                    IsRelative = true
                };
            }
        }
    }
}
```

**Important patterns for the control system:**
- Always gate on `_turnManager.CurrentPhase != TurnManager.Phase.TickUpdate` — decisions only happen in TickUpdate
- Include `CommandTokenComponent` in the entity filter so only actors with available actions get processed
- Use `OptionalComponent<MoveCommandComponent>` with `.Ensure()` to create the command
- Set `IsRelative = true` for directional movement

### 5. Register the system in GameMainContainer

In `PavEcsLiteGame/GameLoop/GameMainContainer.cs`, add the system in the TickUpdate-gated section (after `KeyboardMoveSystem`, before `MoveCommandSystem`):

```csharp
.Add(new $0ControlSystem(turnManager, _systems))
```

### 6. Place the actor on the map

Either:
- Add spawn markers in map data files (`PavEcsGame.Common/Data/`) — existing map loading will create `SpawnRequestComponent` entities
- Or create spawn requests programmatically in `LoadMapSystem`

### Verification

Build the solution to verify the source generator produces correct code:
```bash
dotnet build PavEcsGame.sln
```

## Checklist

- [ ] EntityType enum value added
- [ ] Entity struct defined with proper composition
- [ ] Spawn case handled in TrySpawnEntity with symbol, color, friction, tokens
- [ ] Control system created with TickUpdate phase gate and CommandToken filter
- [ ] System registered in GameMainContainer in correct order
- [ ] Solution builds successfully