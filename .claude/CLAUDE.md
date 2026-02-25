# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

**Build outputs:**
- Executable: `PavEcsLiteGame/bin/Debug/net10.0/PavEcsGame.Lite` (or .exe on Windows)
- Data files copied to: `PavEcsLiteGame/bin/Debug/net10.0/Data/` (from PavEcsGame.Common/.csproj CopyToOutputDirectory)

### Build Commands

```bash
# Build entire solution
dotnet build PavEcsGame.sln

# Build only the main game
dotnet build PavEcsLiteGame/PavEcsGame.Lite.csproj

# Build only the source generator
dotnet build PavEcsSpec.Generators/PavEcsSpec.Generators.csproj

# Build the generator test project (validates generated code)
dotnet build GenerateTest/GenerateTest.csproj
```

### Run with dotnet run (recommended for development)

File paths are resolved automatically via `FileHelper.ResolvePath()`:
- Relative paths are first checked against CWD
- If not found, fallback to executable directory (where `dotnet run` places files)

```bash
# Run with default map (Data/lightTest.txt)
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj

# Run with specific map from repo root (relative or absolute path)
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj -m PavEcsGame.Common/Data/lightTest.txt

# Run with custom map file
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj -m /path/to/custom_map.txt

# Run with ambient light (silver/light gray tint on all tiles)
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj -ambient_light C0C0C0

# Generate map using WCF from pattern
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj -g PavEcsGame.Common/Data/lightTest.txt PavEcsGame.Common/Data/wcf_pattern_test.txt
```

### Run from build output (direct binary execution)

```bash
# Build first
dotnet build PavEcsLiteGame/PavEcsGame.Lite.csproj

# Run from output directory (Data/ files are in bin/Debug/net10.0/Data/)
cd PavEcsLiteGame/bin/Debug/net10.0 && ./PavEcsGame.Lite

# Run with specific map (path relative to bin/Debug/net10.0/)
cd PavEcsLiteGame/bin/Debug/net10.0 && ./PavEcsGame.Lite -m Data/lighting_test_map.txt

# Run map generation mode
cd PavEcsLiteGame/bin/Debug/net10.0 && ./PavEcsGame.Lite -g Data/lightTest.txt Data/wcf_pattern_test.txt
```

There is no test framework (no xUnit/NUnit/MSTest). The `GenerateTest` project serves as a compile-time validation that the Roslyn source generator produces correct code.

**Note:** The game requires an interactive console (reads keyboard input). Cannot run in background.

## CLI Arguments

```
-m <mapFile>              Load and run a specific map file
-g <mapFile> <pattern>    Generate map using WCF from pattern
-ambient_light <hex>      Set ambient light color (hex RGB, e.g., C0C0C0 for silver)
(no args)                 Loads default map: Data/lightTest.txt
```

### Path Resolution for Custom Maps

The `FileHelper.ResolvePath()` utility (in `PavEcsGame.Common/Utils/`) enables flexible map loading:

1. **Absolute paths** — Used as-is
2. **CWD-relative paths** — Used if they exist (supports running from repo root with `-m PavEcsGame.Common/Data/custom.txt`)
3. **Fallback to executable directory** — For `dotnet run`, paths resolve to `bin/Debug/net10.0/Data/`

This means:
- `dotnet run ... -m Data/custom.txt` works from repo root (fallback)
- `dotnet run ... -m PavEcsGame.Common/Data/custom.txt` works from repo root (CWD check)
- `./PavEcsGame.Lite -m Data/custom.txt` works from `bin/Debug/net10.0/` (CWD check)

**Pattern for future work:** When wrapping file I/O for CWD-independent paths, use this three-tier strategy:
1. Check if path is absolute → use as-is
2. Check if exists relative to CWD → use as-is
3. Fallback to entry point assembly directory → for `dotnet run` artifact resolution

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

**Note on .NET compatibility:** PavEcsGame.Common targets `netstandard2.1`. Avoid C# 9.0+ features (relational patterns, required keyword) and APIs added after netstandard2.1 (e.g., `AppContext.BaseDirectory`). Use `Assembly.GetEntryAssembly()?.Location` instead for cross-platform path resolution.

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
- **Ambient Light** — `AmbientLightComponent` sets base illumination color for all tiles (via CLI `-ambient_light <hex>` or `CommandSystem.SetAmbientLight()`); applied when map loads via `LightRenderSystem` using map revision tracking
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
- `PavEcsGame.Common/Utils/` — Helper methods (`FileHelper.ResolvePath()`, `GetByRate`, `GetByRateLerp`, etc.)
- `PavEcsSpec.EcsLite/Spec/` — ECS abstraction (`EcsSpec`, `EcsUniverse`, `EcsFilterSpec`)