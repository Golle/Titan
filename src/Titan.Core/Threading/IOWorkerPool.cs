using System.Collections.Concurrent;
using Titan.Core.Logging;

// ReSharper disable InconsistentNaming

namespace Titan.Core.Threading
{

    // TODO: can we make this use less heap allocated GC memory?
    public static class IOWorkerPool
    {
        private static Thread[] _threads;
        private static bool _active;
        private static readonly Semaphore Semaphore = new(0, int.MaxValue);
        private static readonly ConcurrentQueue<IOThreadWorkerItem> _workerItems = new();

        public static void Init(int threadCount = 4, int maxQueuedJobs = 1000)
        {
            if (_threads != null)
            {
                throw new InvalidOperationException($"{nameof(IOWorkerPool)} has already been initialized.");
            }

            Logger.Trace($"Creating {threadCount} IO threads with priority {ThreadPriority.BelowNormal}", typeof(IOWorkerPool));
            Logger.Trace($"Creating a queue with a max limit of {maxQueuedJobs}", typeof(IOWorkerPool));

            _threads = new Thread[threadCount];
            _active = true;
            for (var i = 0; i < threadCount; i++)
            {
                var thread = _threads[i] = new Thread(Run);
                thread.Name = $"IOThread #{i}";
                thread.Priority = ThreadPriority.BelowNormal;
                thread.IsBackground = true;
                thread.Start(i);
            }
        }

        public static void Terminate()
        {
            if (_active)
            {
                _active = false;
                Semaphore.Release(_threads.Length);
                foreach (var thread in _threads)
                {
                    thread.Join();
                }
                _threads = null;
            }
        }

        private static void Run(object obj)
        {
            var threadId = (int)obj;
            while (_active)
            {
                Semaphore.WaitOne();
                if (_workerItems.TryDequeue(out var workerItem))
                {
                    workerItem.Execute();
                }
            }
        }

        public static void QueueWorkerItem<T>(Action<T> action, in T state)
        {
            _workerItems.Enqueue(new IOThreadWorkerItem<T>(action, state));
            Semaphore.Release();
        }
    }
}
