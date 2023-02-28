using System.Threading;
using Leopotam.EcsLite;
using PavEcsGame.Utils;

namespace PavEcsGame.Systems
{
    internal class SynchronizationContextSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly WorkQueue _workQueue;

        public SynchronizationContextSystem()
        {
            _workQueue = new WorkQueue();
        }

        public void Init(IEcsSystems systems)
        {
            SynchronizationContext.SetSynchronizationContext(new EcsSynchronizationContext(_workQueue));
        }

        public void Run(IEcsSystems systems)
        {
            _workQueue.RunEqueuedOnly();
        }
    }
}
