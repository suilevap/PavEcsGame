using System;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Components.Events;
using PavEcsGame.Systems.Utils;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    internal partial class DebugLogPanelSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly GameDebugLog _debugLog;
        private string[] _debugLines = Array.Empty<string>();
        private int _lastLogVersion;
        private int _mapHeight;
        private EcsPackedEntityWithWorld _panelEntity;

        private readonly Providers _providers;

        [Entity]
        private readonly partial struct MapLoadedEnt
        {
            public partial ref readonly MapLoadedEvent Loaded();
        }

        [Entity]
        private readonly partial struct WindowResizeEnt
        {
            public partial ref readonly WindowResizeEvent Resize();
        }

        [Entity(SkipFilter = true)]
        private readonly partial struct TextPanelEnt
        {
            public partial ref TextPanelComponent Panel();
        }

        public DebugLogPanelSystem(EcsSystems systems, GameDebugLog debugLog)
            : this(systems)
        {
            _debugLog = debugLog;
        }

        public void Init(IEcsSystems systems)
        {
            var newEnt = _providers.TextPanelEntProvider.New();
            newEnt.Panel() = TextPanelComponent.Create(row: 0, col: 0, width: 1, lineCount: 1);
            _panelEntity = newEnt.Id;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in _providers.MapLoadedEntProvider)
            {
                _mapHeight = ent.Loaded().Size.Y;
                if (_providers.TextPanelEntProvider.TryGet(_panelEntity, out var panelEnt))
                {
                    var panelHeight = Math.Max(1, Console.WindowHeight - _mapHeight);
                    TextPanelComponent.Resize(ref panelEnt.Panel(), row: _mapHeight, col: 0, width: Console.WindowWidth, lineCount: panelHeight);
                    _lastLogVersion = -1;
                }
            }

            foreach (var ent in _providers.WindowResizeEntProvider)
            {
                var resize = ent.Resize();
                if (_panelEntity.IsAlive() && _mapHeight > 0)
                {
                    if (_providers.TextPanelEntProvider.TryGet(_panelEntity, out var panelEnt))
                    {
                        var panelHeight = Math.Max(1, resize.Height - _mapHeight);
                        TextPanelComponent.Resize(ref panelEnt.Panel(), row: _mapHeight, col: 0, width: resize.Width, lineCount: panelHeight);
                        _lastLogVersion = -1;
                    }
                }
            }

            if (_panelEntity.IsAlive() && _lastLogVersion != _debugLog.Version)
            {
                if (_providers.TextPanelEntProvider.TryGet(_panelEntity, out var panelEnt))
                {
                    _lastLogVersion = _debugLog.Version;
                    ref var panel = ref panelEnt.Panel();
                    var height = panel.LineCount;
                    if (_debugLines.Length < height)
                        _debugLines = new string[height];
                    var lines = _debugLines;
                    _debugLog.GetLastLines(height, lines, out var count);

                    TextPanelComponent.Clear(ref panel);
                    for (int i = 0; i < count; i++)
                        TextPanelComponent.SetLine(ref panel, i, lines[i]);
                }
            }
        }
    }
}
