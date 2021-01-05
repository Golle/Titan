using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;

namespace Titan.Core.Threading
{
    public record WorkerPoolConfiguration(uint MaxQueuedJobs, uint NumberOfWorkers);

    public sealed class WorkerPool : IDisposable
    {
        private WorkerInfo[] _workers;
        private Job[] _jobs;
        private Semaphore _notifier; // TODO: Replace with a custom Semaphore/Notifier later. But for now a Semaphore is enough
        private ConcurrentQueue<int> _jobQueue;
        private volatile int _nextJob;
        private int _maxJobs;


        public void Initialize(WorkerPoolConfiguration configuration)
        {
            if (configuration.NumberOfWorkers >= Environment.ProcessorCount)
            {
                LOGGER.Warning("Number of Threads are greater or equal to the number of logical processors in the machine. {0} >= {1}", configuration.NumberOfWorkers, Environment.ProcessorCount);
            }

            if (_workers != null)
            {
                throw new InvalidOperationException($"{nameof(WorkerPool)} has already been initialized.");
            }

            LOGGER.Debug("Creating {0} worker threads.", configuration.NumberOfWorkers);
            LOGGER.Debug("Creating job queue with size {0}", configuration.MaxQueuedJobs);

            _maxJobs = (int) configuration.MaxQueuedJobs;
            _jobs = new Job[configuration.MaxQueuedJobs];
            _jobQueue = new ConcurrentQueue<int>();

            _notifier = new Semaphore(0, (int) configuration.MaxQueuedJobs);

            _workers = new WorkerInfo[configuration.NumberOfWorkers];
            for (var i = 0; i < configuration.NumberOfWorkers; ++i)
            {
                ref var worker = ref _workers[i];
                worker = new WorkerInfo
                {
                    Active = true,
                    State = WorkerState.Running,
                    Thread = new Thread(RunWorker)
                    {
                        Priority = ThreadPriority.AboveNormal,
                        Name = $"Worker #{i}"
                    }
                };
                worker.Thread.Start(i);
            }
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(in JobDescription description) => Enqueue(description, null);
        public void Enqueue(in JobDescription description, JobProgress progress)
        {
            var jobIndex = GetNextJob();

            ref var job = ref _jobs[jobIndex];
            job.OnComplete = description.OnComplete;
            job.OnExecute = description.OnExecute;
            job.AutoReset = description.AutoReset;
            job.Progress = progress;
            
            _jobQueue.Enqueue(jobIndex);
            _notifier.Release();
        }

        private void RunWorker(object obj)
        {
            var index = (int) obj;
            ref var worker = ref _workers[index];
            worker.State = WorkerState.Running;
            while (worker.Active)
            {
                worker.State = WorkerState.Wait;
                _notifier.WaitOne();
                worker.State = WorkerState.Running;
                if (!_jobQueue.TryDequeue(out var jobIndex))
                {
                    continue;
                }

#pragma warning disable 420 // TODO: investigate why this is needed.
                ref var job = ref _jobs[jobIndex];
                Volatile.Write(ref job.State, JobState.Running);
                job.State = JobState.Running;
                job.OnExecute();
                Volatile.Write(ref job.State, JobState.Completed);
                job.OnComplete?.Invoke();
                job.Progress?.JobComplete();
                if (job.AutoReset)
                {
                    Volatile.Write(ref job.State, JobState.Available);
                }
#pragma warning restore 420                
            }
        }

        private int GetNextJob()
        {
            // TODO: if the queue is "full" for some reason (maybe AutoReset = false), this method will never return. How should it be handled?
            while (true)
            {
                var current = _nextJob;
                var index = Interlocked.CompareExchange(ref _nextJob, (current + 1) % _maxJobs, current);
                // Some other thread updated the counter, do another lap
                if (index != current)
                {
                    continue;
                }

                var previousStatus = Interlocked.CompareExchange(ref _jobs[index].State, JobState.Waiting, JobState.Available);
                // If the job is busy, loop again and try to find a new spot
                if (previousStatus != JobState.Available)
                {
                    continue;
                }
                return index;
            }
        }

        ~WorkerPool() => Dispose();
        public void Dispose()
        {
            if (_workers != null)
            {
                GC.SuppressFinalize(this);
                // Set the worker Active = false
                for (var i = 0; i < _workers.Length; ++i)
                {
                    ref var worker = ref _workers[i];
                    worker.Active = false;
                }
                // Notify all workers so they'll leave the Wait
                _notifier.Release(_workers.Length); // TODO: this might throw if the Worker is busy. Make sure it doesn't

                // Join all workers with the main thread
                for (var i = 0; i < _workers.Length; ++i)
                {
                    ref var worker = ref _workers[i];
                    worker.Thread.Join();
                }

                _notifier.Dispose();
                _workers = null;
                _notifier = null;
            }
        }

        private struct WorkerInfo
        {
            internal Thread Thread;
            internal WorkerState State;
            internal bool Active;
        }

        private struct Job
        {
            internal volatile int State;
            internal Action OnExecute;
            internal Action OnComplete;
            internal JobProgress Progress;
            internal bool AutoReset;
        }

        private static class JobState
        {
            internal const int Available = 0;
            internal const int Waiting = 1;
            internal const int Running = 2;
            internal const int Completed = 3;
        }
    }
}
