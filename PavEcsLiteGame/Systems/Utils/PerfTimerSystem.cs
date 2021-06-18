using System;
using System.Diagnostics;
using Leopotam.EcsLite;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    internal class PerfTimerSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private struct PerfTick
        {
            public Stopwatch Watch;
            public string Name;
        }
        private readonly string _name;
        private long _avgTickes;
        private readonly EcsFilterSpec<EcsSpec<PerfTick>, EcsSpec, EcsSpec> _spec;


        public PerfTimerSystem(EcsUniverse universe, string name)
        {
            _name = name;
            universe
                .Register(this)
                .Build(ref _spec);
        }

        public void Run(EcsSystems systems)
        {
            if (_spec.Filter.IsEmpty())
            {
                EcsUnsafeEntity ent = (EcsUnsafeEntity ) _spec.World.NewEntity();
                var watch = new Stopwatch();
                ent.Add(_spec.Include, new PerfTick()
                {
                    Watch = watch,
                    Name = _name
                });
                watch.Start();
            }
            foreach (var ent in _spec.Filter)
            {
                ref var c = ref _spec.Include.Pool1.Get(ent);
                var ms = c.Watch.ElapsedMilliseconds;
                var ticks = c.Watch.ElapsedTicks;
                _avgTickes = (long) (_avgTickes * 0.9f + ticks * 0.1f);
                
                //Debug.WriteLine($"{c.Name} - {_name} : {c.Watch.ElapsedMilliseconds}ms, Avg:{_avgTickes / TimeSpan.TicksPerMillisecond}ms");
                Console.Title = $"Avg:{_avgTickes / TimeSpan.TicksPerMillisecond}ms";
                c.Name = _name;
                c.Watch.Restart();
            }
        }
    }
}
