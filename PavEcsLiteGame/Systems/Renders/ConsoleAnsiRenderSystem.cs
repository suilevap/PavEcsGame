using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    /// <summary>
    /// Simple ANSI console renderer.
    ///
    /// Responsibility: Orchestrate rendering
    /// 1. Collect ECS render commands
    /// 2. Sort by position (scanline order)
    /// 3. Build ANSI buffer
    /// 4. Write to console
    ///
    /// All optimization (color caching, position optimization, cursor auto-advance detection)
    /// is handled by AnsiStringBuilder.
    /// </summary>
    public partial class ConsoleAnsiRenderSystem : IEcsInitSystem, IEcsRunSystem, IEcsSystemSpec
    {
        /// <summary>
        /// Enable sorting render commands by scanline order (Y, then X).
        /// When enabled: Sorts by position (O(n log n) overhead, fewer cursor jumps).
        /// When disabled: Renders in entity order (no sort overhead, assumes already ordered).
        /// Default: false (commands are typically already sorted from ECS).
        /// </summary>
        public bool SortByPosition { get; set; } = false;

        private char[] _ansiBuffer = new char[65536];
        private List<EcsUnsafeEntity> _renderCommandBuffer = new(256);

        [Entity]
        private partial struct RenderCommandEntity
        {
            public partial RequiredComponent<RenderItemCommand> Command();
        }

        public void Init(IEcsSystems systems)
        {
            Console.CursorVisible = false;
        }

        public void Run(IEcsSystems systems)
        {
            BuildAndFlushAnsiBuffer();
        }

        private void BuildAndFlushAnsiBuffer()
        {
            // Step 1: Collect all render commands
            _renderCommandBuffer.Clear();
            foreach (var ent in _providers.RenderCommandEntityProvider)
            {
                _renderCommandBuffer.Add((EcsUnsafeEntity)ent.GetRawId());
            }

            if (_renderCommandBuffer.Count == 0)
                return;

            var commandPool = _providers.RenderCommandEntityProvider._commandPool;

            // Step 2: Optionally sort by scanline order (Y primary, X secondary)
            // Sorting helps AnsiStringBuilder optimize cursor movements by reducing jumps
            if (SortByPosition)
            {
                _renderCommandBuffer.Sort((a, b) =>
                {
                    ref readonly var cmdA = ref commandPool.Get(a);
                    ref readonly var cmdB = ref commandPool.Get(b);
                    int yDiff = cmdA.Position.Value.Y.CompareTo(cmdB.Position.Value.Y);
                    return yDiff != 0 ? yDiff : cmdA.Position.Value.X.CompareTo(cmdB.Position.Value.X);
                });
            }

            // Step 3: Build ANSI string
            // AnsiStringBuilder handles:
            // - State caching (colors, position)
            // - Skipping redundant color commands
            // - Cursor position optimization
            // - Cursor auto-advancement detection
            Span<char> buffer = _ansiBuffer.AsSpan();
            var builder = new AnsiStringBuilder(buffer);

            for (int i = 0; i < _renderCommandBuffer.Count; i++)
            {
                ref readonly var cmd = ref commandPool.Get(_renderCommandBuffer[i]);
                builder.SetCursorIfNeeded(cmd.Position.Value.X, cmd.Position.Value.Y);
                builder.AppendColorRgb(cmd.Symbol.MainColor, cmd.BackgroundColor);

                char symbol = cmd.Symbol.Value != default ? cmd.Symbol.Value : SymbolComponent.Empty.Value;
                builder.AppendChar(symbol);
            }

            // Step 4: Write optimized ANSI buffer to console
            Console.Out.Write(builder.AsSpan());

            // Step 5: Cleanup
            foreach (var entId in _renderCommandBuffer)
            {
                commandPool.Del(entId);
            }
        }
    }
}
