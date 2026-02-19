# AGENTS

This file provides **agentic coding agents** with the information they need to build, lint, test, and style code in this repository.  It is deliberately verbose (≈150 lines) so that no ambiguity remains.

---

## 1. Build / Run Commands

```bash
# Build the entire solution (including all projects and sub‑modules)
# Use this when you change code in any project.
dotnet build PavEcsGame.sln

# Run the main executable (the roguelike game)
# The project is located under PavEcsLiteGame.
dotnet run --project PavEcsLiteGame/PavEcsGame.Lite.csproj

# Build the source‑generator (Roslyn analyzer)
dotnet build PavEcsSpec.Generators/PavEcsSpec.Generators.csproj

# Build the generator‑validation project (Compile‑time check of generated code)
dotnet build GenerateTest/GenerateTest.csproj
```

### Notes
- The solution contains a **Leopotam.EcsLite** vendored submodule (`leo/`).  It is built as part of the solution.
- The `GenerateTest` project has no runtime tests; it simply compiles against the generated code to verify correctness.
- If you add a new test project later, add it to the solution and run `dotnet test`.

---

## 2. Lint / Formatting

| Tool | Command | Purpose |
|------|---------|---------|
| `dotnet format` | `dotnet format --check` | Verify that the code follows the style rules defined in `.editorconfig`. The command fails if any formatting issue is found. |
| `dotnet format` | `dotnet format` | Auto‑format the repository. |

> **Tip**: Configure `dotnet format` by adding a `.editorconfig` file at the repository root.  The current project uses the default C# formatting rules; you may add custom rules for line‑length, naming conventions, etc.

---

## 3. Test Commands (currently none)

The repository does not ship with a test framework – the `GenerateTest` project is only for compile‑time validation.  If you add a test project (e.g., `PavEcsGame.Tests`), use the following pattern:

```bash
# Run all tests in a specific project
# Replace <TestProject> with the path to your .csproj file.
dotnet test <TestProject>

# Run a single test method by fully‑qualified name
# Example: PavEcsGame.Tests.Unit.MyComponentTests.ShouldCreatePosition

dotnet test <TestProject> --filter FullyQualifiedName~PavEcsGame.Tests.Unit.MyComponentTests.ShouldCreatePosition
```

---

## 4. Code‑Style Guidelines

### 4.1 Imports / Using Directives
- Sort alphabetically.
- Group into three blocks:
  1. `System` namespaces
  2. Third‑party (e.g., `Leopotam.EcsLite`, `PavEcsSpec.Generated`)
  3. Project namespaces (`PavEcsGame.*`).
- Remove unused `using`s – enable the *Remove Unused Usings* analyzer.

### 4.2 Formatting
- 4‑space indentation; no tabs.
- Opening braces on the same line: `if (...) {`.
- Single space after control keywords (`if`, `for`, `while`).
- No trailing whitespace.
- Line length ≤ 120 characters; longer lines may be split with the `//` continuation comment.
- XML documentation comments use `<summary>`, `<param>`, `<returns>` tags for public APIs.

### 4.3 Naming Conventions
| Element | Convention |
|---------|------------|
| **Public types, methods, properties** | `PascalCase` |
| **Private fields** | `_camelCase` (leading underscore) |
| **Local variables, parameters** | `camelCase` |
| **Constants / readonly static fields** | `UPPER_SNAKE_CASE` |
| **Enums** | `PascalCase` |

#### Component Naming (from `.claude/skills/new-component/SKILL.md`)
- **Data components**: `<Name>Component` (e.g., `PositionComponent`).  Stored in `PavEcsGame.Components/Components`.
- **Tag components**: `<Name>Tag`.  Stored in `PavEcsGame.Components/Components/Tags` and implement `IEcsIgnoreInFilter`.
- **Event components**: `<Name>Event`.  Stored in `PavEcsGame.Components/Components/Events`.
- **System‑internal components**: `<Name>Component` under `PavEcsGame.Components/Components/SystemComponents`.
- **Namespace**: All components belong to `PavEcsGame.Components` (or sub‑namespace for events).

### 4.4 Error Handling
- Guard clauses at the beginning of public methods: `if (arg == null) throw new ArgumentNullException(nameof(arg));`.
- Prefer explicit exceptions (`ArgumentOutOfRangeException`, `InvalidOperationException`) over swallowed errors.
- Use `using` for disposable resources.

### 4.5 Nullability
- Enable nullable reference types in all projects: `<Nullable>enable</Nullable>` in `.csproj`.
- Prefer `?` for nullable references and use `!` only when you are absolutely certain a value is non‑null.
- Use `[MaybeNullWhen(false)]` or similar attributes where appropriate.

### 4.6 Documentation
- All **public** types, methods, properties must have XML documentation.
- Use `<summary>` to describe the purpose; `<param>` for each parameter; `<returns>` if a value is returned.
- For internal or private APIs, documentation is optional but highly recommended for maintainability.

---

## 5. Project Structure Overview
```
PavEcsLiteGame           – Main executable (net10.0)
├─ Systems/              – All game systems
│  ├─ Renders/
│  ├─ Controls/
│  └─ ...
├─ GameLoop/             – Lifecycle (GameMainContainer)
└─ Extensions/
PavEcsSpec.EcsLite        – ECS abstraction layer
├─ Spec/                  – EcsSpec, EcsUniverse, etc.
└─ ...
PavEcsSpec.Generators    – Roslyn source generator (netstandard2.0)
PavEcsGame.Components   – Pure data structures
├─ Components/            – Component structs
│  ├─ Tags/
│  ├─ Events/
│  └─ SystemComponents/
├─ Interfaces/            – FakeEcsInterfaces.cs
└─ Types/                 – Math types (Int2, etc.)
PavEcsGame.Common        – Shared utilities, map data
GenerateTest              – Compile‑time validation of generated code
```

---

## 6. Cursor / Copilot Rules
There are currently **no** cursor rules (`.cursor/rules/` or `.cursorrules`) and no Copilot instructions (`.github/copilot-instructions.md`).  If you add such rules in the future, place them under the corresponding directories and reference them here.

---

## 7. Additional Notes
- **Debug vs Release**: The README stresses that the framework relies on `DEBUG` checks; always build in `Debug` during development.
- **Thread‑safety**: LeoEcsLite is intentionally not thread‑safe.  Any multithreaded usage must be implemented by the developer.
- **Source generator**: The `PavEcsSpec.Generators` project produces entity provider code.  Do not modify generated files manually – they are overwritten on every build.
- **Extensibility**: New systems, components, and extensions should follow the patterns described above.

---

**End of AGENTS.md**