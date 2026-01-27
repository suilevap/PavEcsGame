using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PavEcsGame.Utils
{
    public class WorkQueue
    {
        private readonly Queue<WorkItem> _queue = new Queue<WorkItem>();

        private readonly struct WorkItem
        {
            public readonly SendOrPostCallback Callback;
            public readonly object State;

            public WorkItem(SendOrPostCallback callback, object state)
            {
                Callback = callback;
                State = state;
            }
        }

        public readonly struct WorkQueueAwaiter : INotifyCompletion
        {
            private readonly WorkQueue _workQueue;

            public WorkQueueAwaiter(WorkQueue workQueue)
            {
                _workQueue = workQueue;
            }

            public bool IsCompleted => false;

            public WorkQueueAwaiter GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                _workQueue.Enqueue(
                    _ => continuation?.Invoke(),
                    null);
            }
        }

        //return specific type instead of interface to avoid boxing
        public WorkQueueAwaiter ContinueInQueue()
        {
            return new WorkQueueAwaiter(this);
        }

        public void Enqueue(SendOrPostCallback callback, object state)
        {
            _queue.Enqueue(new WorkItem(callback, state));
        }

        //execute only items where were enqueued till that moment (to avoid inifinte loops, when item execution add new item)
        public void RunEqueuedOnly()
        {
            var count = _queue.Count;

            while (count > 0 && _queue.Count != 0)
            {
                var item = _queue.Dequeue();
                item.Callback(item.State);
                count--;
            }
        }
    }
}