using Leopotam.Ecs;
using PavEcsGame.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PavEcsGame.Systems
{
    class SynchronizationContextSystem : IEcsInitSystem, IEcsRunSystem
    {
        [EcsIgnoreInject]
        private WorkQueue _workQueue;

        public void Init()
        {
            _workQueue = new WorkQueue();
            SynchronizationContext.SetSynchronizationContext(new EcsSynchronizationContext(_workQueue));
        }

        public void Run()
        {
            _workQueue.RunEqueuedOnly();
        }
    }
}
