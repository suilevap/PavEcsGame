using Leopotam.Ecs;
using PavEcsGame.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Systems
{
    class WorkQueueSystem : IEcsRunSystem
    {
        private WorkQueue _work;

        public void Run()
        {
            _work.RunAll();
        }
    }
}
