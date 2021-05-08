using Leopotam.Ecs;
using PavEcsGame.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Leopotam.EcsLite;

namespace PavEcsGame.Systems
{
    class SynchronizationContextSystem : IEcsInitSystem, IEcsRunSystem
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
