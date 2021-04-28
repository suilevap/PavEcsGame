using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PavEcsGame.Utils
{
    public interface IAwaitable
    {
        IAwaiter GetAwaiter();
    }

    public interface IAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }

        void GetResult();
    }
    public class WorkQueue
    {
        private readonly Queue<Action> _queue = new Queue<Action>();

        public IAwaitable ContinueInRun() => new Awaiter(this);

        public void Enqueue(Action action) => _queue.Enqueue(action);

        //execute only items where were enqueued till that moment (to avoid inifinte loops, when item execution add new item)
        public void RunEqueuedOnly()
        {
            int count = _queue.Count;

            while (count > 0 && _queue.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }

        private class Awaiter : IAwaiter, IAwaitable//INotifyCompletion
        {
            private readonly WorkQueue _workQueue;

            public Awaiter(WorkQueue workQueue)
            {
                _workQueue = workQueue;
            }
            public bool IsCompleted { get; private set; }

            public IAwaiter GetAwaiter() => this;

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                _workQueue.Enqueue(() =>
                {
                    continuation?.Invoke();
                    IsCompleted = true;
                });
            }
        }
    }
}
