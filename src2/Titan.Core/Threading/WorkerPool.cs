using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;

namespace Titan.Core.Threading
{
    public record WorkerPoolConfiguration(uint MaxQueuedJobs, uint NumberOfWorkers);

    public sealed class WorkerPool
    {
        private static WorkerInfo[] _workers;
        private static Job[] _jobs;
        private static Semaphore _notifier; // TODO: Replace with a custom Semaphore/Notifier later. But for now a Semaphore is enough
        private static ConcurrentQueue<int> _jobQueue;
        private static volatile int _nextJob;
        private static int _maxJobs;
        public static void Init(WorkerPoolConfiguration configuration)
        {
            if (configuration.NumberOfWorkers >= Environment.ProcessorCount)
            {
                Logger.Warning<WorkerPool>($"Number of Threads are greater or equal to the number of logical processors in the machine. {configuration.NumberOfWorkers} >= {Environment.ProcessorCount}");
            }

            if (_workers != null)
            {
                throw new InvalidOperationException($"{nameof(WorkerPool)} has already been initialized.");
            }

            Logger.Debug<WorkerPool>($"Creating {configuration.NumberOfWorkers} worker threads.");
            Logger.Debug<WorkerPool>($"Creating job queue with size {configuration.MaxQueuedJobs}");

            _maxJobs = (int) configuration.MaxQueuedJobs;
            _jobs = new Job[configuration.MaxQueuedJobs + 1]; // Index 0 will be invalid, so we create the array with one more element
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
        public static bool IsCompleted(in Handle<WorkerPool> handle)
        {
            Debug.Assert(handle.IsValid(), "Invalid handle");
            ref var job = ref _jobs[handle.Value];
            if (job.AutoReset)
            {
                ThrowException();
            }
            return job.State == JobState.Completed;
            static void ThrowException() => throw new InvalidOperationException($"{nameof(IsCompleted)} is only supported when {nameof(Job.AutoReset)} is set to false");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(ref Handle<WorkerPool> handle)
        {
            ref var job = ref _jobs[handle.Value];
            if (job.AutoReset || job.State != JobState.Completed)
            {
                ThrowException();
            }
#pragma warning disable 420 // TODO: investigate why this is needed.
            Volatile.Write(ref job.State, JobState.Available);
#pragma warning restore 420

            handle = default; // Invalidate the Handle
            static void ThrowException() => throw new InvalidOperationException($"{nameof(Reset)} can only be called on jobs with {nameof(Job.AutoReset)} set to false and that has been finished. Call {nameof(IsCompleted)} to check if the job has finished.");
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Handle<WorkerPool> Enqueue(in JobDescription description) => Enqueue(description, null);
        public static Handle<WorkerPool> Enqueue(in JobDescription description, JobProgress progress)
        {
            var jobIndex = GetNextJob();

            ref var job = ref _jobs[jobIndex];
            job.OnComplete = description.OnComplete;
            job.OnExecute = description.OnExecute;
            job.AutoReset = description.AutoReset;
            job.Progress = progress;
            job.Metadata = description.Metadata;
            
            _jobQueue.Enqueue(jobIndex);
            _notifier.Release();
            
            return jobIndex;
        }

        private static void RunWorker(object obj)
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
                
                var progress = job.Progress;
                Volatile.Write(ref job.State, JobState.Running);
                job.OnExecute();
                job.OnComplete?.Invoke(job.Metadata);
                Volatile.Write(ref job.State, JobState.Completed);
                progress?.JobComplete();
                if (job.AutoReset)
                {
                    Volatile.Write(ref job.State, JobState.Available);
                }
#pragma warning restore 420
            }
        }

        private static int GetNextJob()
        {
            // TODO: if the queue is "full" for some reason (maybe AutoReset = false), this method will never return. How should it be handled?
            var maxIterations = _maxJobs;
            while (maxIterations-- > 0)
            {
                var current = _nextJob;
                var index = Interlocked.CompareExchange(ref _nextJob, (current + 1) % _maxJobs, current); 
                // Some other thread updated the counter, do another lap
                if (index != current)
                {
                    continue;
                }
                index += 1; // Add 1 so we skip the invalid handle 0

                var previousStatus = Interlocked.CompareExchange(ref _jobs[index].State, JobState.Waiting, JobState.Available);
                // If the job is busy, loop again and try to find a new spot
                if (previousStatus != JobState.Available)
                {
                    continue;
                }
                return index;
            }
            ThrowException(_maxJobs);
            static void ThrowException(int maxIterations)
            {
                throw new InvalidOperationException($"MaxIterations to enqueue a job was exceeded. This could be happening due to AutoReset = false on some jobs and they are not being reset. Max Iterations = {maxIterations}");
            }
            return -1;
        }

        public static void Terminate()
        {
            if (_workers != null)
            {
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

                for (var i = 0; i < _jobs.Length; ++i)
                {
                    _jobs[i] = default;
                }

                _notifier.Dispose();
                _workers = null;
                _notifier = null;
                _jobs = null;
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
            internal Action<int> OnComplete;
            internal volatile JobProgress Progress;
            internal bool AutoReset;
            internal int Metadata;
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
