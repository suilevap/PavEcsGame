using System;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    internal partial class TextPanelRenderSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private char[] _ansiBuffer = new char[65536];

        private readonly Providers _providers;

        [Entity]
        private readonly partial struct TextPanelEnt
        {
            public partial ref readonly TextPanelComponent Panel();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in _providers.TextPanelEntProvider)
            {
                ref readonly var panel = ref ent.Panel();
                EnsureAnsiBuffer(in panel);
                var builder = new AnsiStringBuilder(_ansiBuffer.AsSpan());
                for (int i = 0; i < panel.LineCount; i++)
                {
                    var text = panel.Lines[i].AsSpan();
                    var fg = panel.FgColors[i];
                    var bg = panel.BgColors[i];
                    builder.AppendLine(panel.Col, panel.Row + i, panel.Width, text, fg, bg);
                }
                Console.Out.Write(builder.AsSpan());
            }
        }

        private void EnsureAnsiBuffer(in TextPanelComponent panel)
        {
            int needed = panel.LineCount * (panel.Width + 30);
            if (_ansiBuffer.Length < needed)
                _ansiBuffer = new char[needed];
        }
    }
}
