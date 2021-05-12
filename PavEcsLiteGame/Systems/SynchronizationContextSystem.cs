using System.Threading;
using Leopotam.EcsLite;
using PaveEcsGame.Utils;

namespace PavEcsGame.Systems
{
    internal class SynchronizationContextSystem : IEcsInitSystem, IEcsRunSystem
    {
        private WorkQueue _workQueue;

        public void Init(EcsSystems systems)
        {
            _workQueue = new WorkQueue();
            SynchronizationContext.SetSynchronizationContext(new EcsSynchronizationContext(_workQueue));
        }

        public void Run(EcsSystems systems)
        {
            _workQueue.RunEqueuedOnly();
        }
    }
}
