using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Leopotam.EcsLite;

namespace PavEcsGame.Systems.Utils
{
    internal class PerformanceMonitoringEcsSystems : EcsSystems
    {
        private class PerfStats
        {
            public long TotalMs { get; set; }
            public long MinMs { get; set; }
            public long MaxMs { get; set; }
            public long CallCount { get; set; }

            public double AverageMs => CallCount > 0 ? TotalMs / (double)CallCount : 0;

            public PerfStats()
            {
                TotalMs = 0;
                MinMs = long.MaxValue;
                MaxMs = 0;
                CallCount = 0;
            }

            public void Record(long elapsedMs)
            {
                TotalMs += elapsedMs;
                MinMs = Math.Min(MinMs, elapsedMs);
                MaxMs = Math.Max(MaxMs, elapsedMs);
                CallCount++;
            }
        }

        private class RunSystemWrapper : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem, IEcsPreInitSystem, IEcsPostRunSystem, IEcsPostDestroySystem
        {
            private readonly IEcsRunSystem _inner;
            private readonly string _systemName;
            private readonly PerfStats _stats;
            private readonly Stopwatch _stopwatch;

            public PerfStats Stats => _stats;
            public string SystemName => _systemName;

            public RunSystemWrapper(IEcsRunSystem system)
            {
                _inner = system;
                _systemName = system.GetType().Name;
                _stats = new PerfStats();
                _stopwatch = new Stopwatch();
            }

            public void Run(IEcsSystems systems)
            {
                _stopwatch.Restart();
                _inner.Run(systems);
                _stopwatch.Stop();
                _stats.Record(_stopwatch.ElapsedMilliseconds);
            }

            void IEcsInitSystem.Init(IEcsSystems systems)
            {
                if (_inner is IEcsInitSystem initSystem)
                    initSystem.Init(systems);
            }

            void IEcsDestroySystem.Destroy(IEcsSystems systems)
            {
                if (_inner is IEcsDestroySystem destroySystem)
                    destroySystem.Destroy(systems);
            }

            void IEcsPreInitSystem.PreInit(IEcsSystems systems)
            {
                if (_inner is IEcsPreInitSystem preInitSystem)
                    preInitSystem.PreInit(systems);
            }

            void IEcsPostRunSystem.PostRun(IEcsSystems systems)
            {
                if (_inner is IEcsPostRunSystem postRunSystem)
                    postRunSystem.PostRun(systems);
            }

            void IEcsPostDestroySystem.PostDestroy(IEcsSystems systems)
            {
                if (_inner is IEcsPostDestroySystem postDestroySystem)
                    postDestroySystem.PostDestroy(systems);
            }
        }

        private readonly List<RunSystemWrapper> _wrappers;
        private readonly bool _enableProfiling;
        private DateTime _lastReport;
        private readonly TimeSpan _reportInterval;
        private readonly double _minAvgMsToShow;

        public PerformanceMonitoringEcsSystems(EcsWorld world, string name, bool enableProfiling = true)
            : base(world, name)
        {
            _wrappers = new List<RunSystemWrapper>();
            _enableProfiling = enableProfiling;
            _reportInterval = TimeSpan.FromSeconds(5);
            _minAvgMsToShow = 0.01;
            _lastReport = DateTime.UtcNow;
        }

        public override IEcsSystems Add(IEcsSystem system)
        {
            if (!_enableProfiling)
            {
                return base.Add(system);
            }

            if (system is IEcsRunSystem runSystem)
            {
                var wrapper = new RunSystemWrapper(runSystem);
                _wrappers.Add(wrapper);
                return base.Add(wrapper);
            }

            return base.Add(system);
        }

        public override void Run()
        {
            base.Run();

            if (!_enableProfiling)
            {
                return;
            }

            if (DateTime.UtcNow - _lastReport > _reportInterval)
            {
                PrintStats();
                _lastReport = DateTime.UtcNow;
            }
        }

        private void PrintStats()
        {
            var filtered = _wrappers
                .Where(w => w.Stats.AverageMs >= _minAvgMsToShow)
                .OrderByDescending(w => w.Stats.AverageMs)
                .ToList();

            if (filtered.Count == 0)
            {
                return;
            }

            var output = new System.Text.StringBuilder();
            output.AppendLine("═══════════════════════════════════════════════════════════════════");
            output.AppendLine("                    ECS PERFORMANCE REPORT");
            output.AppendLine("═══════════════════════════════════════════════════════════════════");
            output.AppendLine("System                           | Avg(ms) | Min | Max | Total");
            output.AppendLine("─────────────────────────────────┼─────────┼─────┼─────┼────────");

            double totalMs = 0;
            foreach (var wrapper in filtered)
            {
                var stats = wrapper.Stats;
                totalMs += stats.TotalMs;

                var systemName = wrapper.SystemName.Length > 31
                    ? wrapper.SystemName.Substring(0, 31)
                    : wrapper.SystemName.PadRight(31);

                output.AppendLine(string.Format(
                    "{0} | {1,7:F2} | {2,3} | {3,3} | {4,7:F1}",
                    systemName,
                    stats.AverageMs,
                    stats.MinMs,
                    stats.MaxMs,
                    stats.TotalMs));
            }

            output.AppendLine("─────────────────────────────────┴─────────┴─────┴─────┴────────");
            output.AppendLine(string.Format("Total: {0:F1}ms across {1} systems", totalMs, filtered.Count));
            output.AppendLine("═══════════════════════════════════════════════════════════════════");

            Debug.Print(output.ToString());
        }
    }
}
