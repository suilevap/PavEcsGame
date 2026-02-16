using System;
using Leopotam.EcsLite;
using PavEcsGame.Components.Events;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems.Renders
{
    public partial class WindowResizeDetectionSystem : IEcsRunSystem, IEcsPostRunSystem, IEcsSystemSpec
    {
        private int _lastWidth;
        private int _lastHeight;

        private readonly Providers _providers;

        [Entity(SkipFilter = true)]
        private readonly partial struct WindowResizeEnt
        {
            public partial ref WindowResizeEvent Event();
        }

        [Entity]
        private readonly partial struct CleanupWindowResizeEnt
        {
            public partial RequiredComponent<WindowResizeEvent> Event();
        }

        public void Run(IEcsSystems systems)
        {
            var w = Console.WindowWidth;
            var h = Console.WindowHeight;
            if (w == _lastWidth && h == _lastHeight)
                return;

            _lastWidth = w;
            _lastHeight = h;

            Console.Clear();

            _providers.WindowResizeEntProvider
                .New()
                .Event() = new WindowResizeEvent { Width = w, Height = h };
        }

        public void PostRun(IEcsSystems systems)
        {
            // Clean up WindowResizeEvent created in Run()
            foreach (var ent in _providers.CleanupWindowResizeEntProvider)
            {
                ent.Event().Remove();
            }
        }
    }
}
