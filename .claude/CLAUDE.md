# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build entire solution
dotnet build PavEcsGame.sln

# Build and run the main game (PavEcsLiteGame)
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj

# Build and run the main game with specific map (PavEcsLiteGame)
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj -m PavEcsGame.Common/Data/lightTest.txt

# Build and run the main game with generation map (PavEcsLiteGame)
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj -g PavEcsGame.Common/Data/lightTest.txt PavEcsGame.Common/Data/wcf_pattern_test.txt 

# Run from build output (required for Data/ folder access)
cd PavEcsLiteGame/bin/Debug/net10.0 && ./PavEcsGame.Lite

# Build only the source generator
dotnet build PavEcsSpec.Generators/PavEcsSpec.Generators.csproj

# Build the generator test project (validates generated code)
dotnet build GenerateTest/GenerateTest.csproj

# Run with specific map
cd PavEcsLiteGame/bin/Debug/net10.0 && ./PavEcsGame.Lite -m Data/lighting_test_map.txt

# Run map generation mode
cd PavEcsLiteGame/bin/Debug/net10.0 && ./PavEcsGame.Lite -g Data/lightTest.txt Data/wcf_pattern_test.txt
```

There is no test framework (no xUnit/NUnit/MSTest). The `GenerateTest` project serves as a compile-time validation that the Roslyn source generator produces correct code.

**Note:** The game requires an interactive console (reads keyboard input). Cannot run in background or via `dotnet run` from project directory - must run the executable from the build output directory where Data/ files are copied.

## CLI Arguments

```
-m <mapFile>              Load and run a specific map file
-g <mapFile> <pattern>    Generate map using WCF from pattern
(no args)                 Loads default map: Data/lightTest.txt
```

## Architecture

This is a **roguelike dungeon crawler** built on the **Entity-Component-System** pattern using [Leopotam EcsLite](https://github.com/Leopotam/ecslite) (vendored in `leo/` as a git submodule).

### Project Dependency Graph

```
PavEcsLiteGame (net10.0, main executable)
├── PavEcsSpec.EcsLite (ECS abstraction layer)
│   └── Leopotam.EcsLite (leo/)
├── PavEcsSpec.Generators (Roslyn source generator, netstandard2.0)
├── PavEcsGame.Components (component struct definitions)
│   └── (no ECS dependency - pure data)
└── PavEcsGame.Common (shared utilities, map data, tiles)
    └── PavEcsGame.Components

GenerateTest (net10.0, generator validation)
├── Leopotam.EcsLite
├── PavEcsGame.Components
├── PavEcsGame.Common
└── PavEcsSpec.Generators (as Analyzer)
```

- **PavEcsGame/** is a legacy project (netcoreapp3.1, references old `ecs/` not `leo/`). Active development is in **PavEcsLiteGame/**.

### Roslyn Source Generator (`PavEcsSpec.Generators`)

The generator targets `netstandard2.0` and is referenced as an `Analyzer` (not a normal project reference) in consuming projects. It processes `[Entity]` attributes on partial structs inside systems and generates:

- **Entity ref structs** with component accessors (ref, ref readonly, Optional, Required)
- **Provider classes** with `EcsPool<T>` fields, `EcsFilter`, enumerator, and `New()`/`Get()`/`TryGet()` factory methods
- **System partial classes** with a `Providers` struct that wires up all entity providers
- **Infrastructure types**: `OptionalComponent<T>`, `RequiredComponent<T>`, `ExcludeComponent<T>`, `EntityAttribute`

Key generator files:
- `SystemSetGenerator.cs` — Main `ISourceGenerator` entry point
- `EntityProviderGenerator.cs` — Generates Provider code per entity type
- `EcsEntityDescriptor.cs` — Parses entity struct declarations

### System Pattern

Systems implement `IEcsRunSystem`/`IEcsInitSystem` and `IEcsSystemSpec`. They are `partial` classes containing `[Entity]`-attributed partial structs:

```csharp
public partial class MySystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec {
    private readonly Providers _providers;  // Generated

    [Entity]
    private readonly partial struct MyEntity {
        public partial ref readonly PositionComponent Pos();
        public partial ref SpeedComponent Speed();
        public partial OptionalComponent<NewPositionComponent> NewPos();
    }

    public void Run(IEcsSystems systems) {
        foreach (var entity in _providers.MyEntityProvider) { ... }
    }
}
```

Return types on entity methods control access:
- `ref T` — read/write pool access
- `ref readonly T` — read-only pool access (filter include)
- `OptionalComponent<T>` — optional (not in filter)
- `RequiredComponent<T>` — required but through wrapper
- No ref (value return) — also used for nested entity composition

### EcsUniverse (Multi-World)

`EcsUniverse` distributes components across multiple `EcsWorld` instances using a union-find algorithm. Systems that share components are assigned to the same world. This isolates unrelated component groups for performance.

### Entity Composition

Entity types compose via nesting — an entity method returning another entity type pulls in all its components:

```csharp
[Entity] struct ActorEntity { public partial PhysicEntity Physic(); ... }
[Entity] struct PlayerEntity { public partial ActorEntity Actor(); public partial LightEntity Light(); ... }
```

### Turn-Based Game Loop

`GameMainContainer` creates systems and runs `while (game.IsAlive) game.Update()`. The turn system uses a two-phase approach orchestrated by `TurnManager`:

**Phases:**
- **TickUpdate** — Active when no simulation work is pending. Input/decision systems run here (keyboard, AI).
- **Simulation** — Active when systems report work to do. Physics/resolution systems run here (movement, collision, destruction).

**Token-based action economy:**
1. `CommandTokenDistributionSystem` recharges actors: copies `WaitCommandTokenComponent.RechargeValue` into `CommandTokenComponent.ActionCount` every ~1 second
2. Input systems (only run in TickUpdate) create `MoveCommandComponent` on actors that have tokens
3. `MoveCommandSystem` (Simulation) converts `MoveCommandComponent` → `SpeedComponent`, decrements `CommandTokenComponent.ActionCount`, and reports work to TurnManager
4. Physics pipeline resolves: `MovementSystem` (Speed→NewPosition) → `FrictionSystem` (decay speed) → `UpdatePositionSystem` (collision check, finalize position)
5. When no systems report remaining work, phase returns to TickUpdate

**System execution order** (from `GameMainContainer.Start()`):
1. Infrastructure: TurnManager, CommandSystem, LoadMapSystem, SpawnEntitySystem, TileSystem
2. TickUpdate-gated: CommandTokenDistributionSystem, KeyboardMoveSystem, RandomMoveSystem, MoveCommandSystem
3. Simulation: UpdateDirectionBasedOnSpeedSystem, MovementSystem, FrictionSystem, RelativePositionSystem, UpdatePositionSystem
4. Resolution: DamageOnCollisionSystem, DestroyEntitySystem
5. Visual: DirectionTileSystem, LightSourceSystems, FieldOfViewSystem
6. Rendering: LightRenderSystem, PlayerFieldOfViewSystem, PrepareForRenderSystem, ConsoleAnsiRenderSystem
7. Cleanup: `MyDelHere<T>()` removes temporary components (PreviousPosition, CollisionEvent, etc.)

**Key turn-system components:**
- `WaitCommandTokenComponent` — Defines recharge rate (stored on actor permanently)
- `CommandTokenComponent` — Current available actions (added/removed by distribution system)
- `MoveCommandComponent` — Pending move command with `Target` direction and `IsRelative` flag
- `MoveFrictionComponent` — Friction value that decays speed each simulation step

### Console Rendering

`ConsoleAnsiRenderSystem` collects `RenderItemCommand` entities, sorts by scanline, and uses `AnsiStringBuilder` (a stack-allocated ref struct with 65KB buffer) to build a single ANSI escape sequence string per frame. Key optimization: cursor auto-advancement detection skips redundant `ESC[row;colH` commands.

### Gradient Lighting System (24-bit RGB)

The lighting system uses smooth RGB gradients instead of 16-color palettes:
- **Gradient palettes** — `LightGradients.cs` defines Color[] arrays (Fire, Electricity, Acid, None) with keyframe colors
- **Interpolation** — `Helper.GetByRateLerp()` maps intensity [0-255] to gradient using `Color.Lerp()`
- **Color blending** — `ToRgbColor()` uses additive RGB blending for overlapping lights (Fire + Electricity = white)
- **ANSI output** — `AnsiStringBuilder.AppendColorRgb()` emits `ESC[38;2;R;G;Bm` (foreground) and `ESC[48;2;R;G;Bm` (background)
- **Critical:** `NoneGradient` MUST start with dark gray (64,64,64), NOT black, to maintain visited area visibility at zero light

**Color type usage:**
- `SymbolComponent.MainColor` — Stores RGB color (was ConsoleColor)
- `RenderItemCommand.BackgroundColor` — Stores RGB background
- `TextPanelComponent.FgColors/BgColors` — UI text colors (Color[] arrays)
- `Color` struct (PavEcsGame.Components/Data/Color.cs) — Has `Lerp()` and clamped `operator+` built-in

## Key Directories

- `PavEcsLiteGame/Systems/` — All game systems (Managers, Renders, Physics, Input, etc.)
- `PavEcsLiteGame/GameLoop/` — Game lifecycle (`GameMainContainer`)
- `PavEcsGame.Components/Components/` — Component structs (Position, Speed, Symbol, Light, etc.)
- `PavEcsGame.Components/Types/` — Math types (`Int2`, geometry)
- `PavEcsGame.Components/Data/` — Data types (`Color` struct with RGB/RGBA)
- `PavEcsGame.Common/Data/` — Map files and wall style definitions (copied to output)
- `PavEcsGame.Common/Utils/` — Helper methods (`GetByRate`, `GetByRateLerp`, etc.)
- `PavEcsSpec.EcsLite/Spec/` — ECS abstraction (`EcsSpec`, `EcsUniverse`, `EcsFilterSpec`)