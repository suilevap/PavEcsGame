using System.Threading;

namespace PaveEcsGame.Utils
{
    public class EcsSynchronizationContext : SynchronizationContext
    {
        private readonly WorkQueue _workQueue;

        public EcsSynchronizationContext(WorkQueue workQueue)
        {
            _workQueue = workQueue;
        }
        public override void Post(SendOrPostCallback d, object state)
        {
            _workQueue.Enqueue(d, state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            _workQueue.Enqueue(d, state);
        }

        public override SynchronizationContext CreateCopy()
        {
            return new EcsSynchronizationContext(_workQueue);
        }

    }
}
