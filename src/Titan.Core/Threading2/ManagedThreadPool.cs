using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;

namespace Titan.Core.Threading2;

public readonly struct ManagedThreadPool : IThreadPoolApi
{

    private const int HandleOffset = 1124521;
    private static Thread[] _threads;
    private static SemaphoreSlim _notifier;

    private static volatile int _nextJob;
    private static volatile int _nextQueuedJob;
    private static volatile int _count;
    private static int _maxJobs;

    private static WorkerInfo[] _workers;
    private static Job[] _jobQueue;

    private static readonly TimeSpan DefaultThreadWaitTime = TimeSpan.FromMilliseconds(100);

    public static void Reset(ref Handle<JobApi> handle)
    {
        Debug.Assert(handle.IsValid());
        var jobIndex = handle.Value - HandleOffset;

        ref var job = ref _jobQueue[jobIndex];
        if (job.State == JobState.Completed)
        {
            job.State = JobState.Available;
        }
        handle = Handle<JobApi>.Null;
    }

    public static void Init(in ThreadPoolConfiguration config)
    {
        _maxJobs = (int)config.MaxJobs;
        _threads = new Thread[config.WorkerThreads];
        _notifier = new SemaphoreSlim(0, _maxJobs);

        _workers = new WorkerInfo[config.WorkerThreads];
        _jobQueue = new Job[config.MaxJobs];

        for (var i = 0; i < _threads.Length; ++i)
        {
            _threads[i] = new Thread(RunWorker)
            {
                Name = $"Worker thread #{i}",
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            _workers[i].Active = true;
            _workers[i].State = WorkerState.Waiting;
            _threads[i].Start(i);
        }
    }

    public static void SignalReady(ReadOnlySpan<Handle<JobApi>> handles)
    {
        var signalCount = 0;
        foreach (var handle in handles)
        {
            if (!handle.IsInvalid())
            {
                ref var job = ref _jobQueue[handle - HandleOffset];
                if (job.State == JobState.Waiting)
                {
                    job.State = JobState.Ready;
                    signalCount++;
                }
            }
        }
        _notifier.Release(signalCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompleted(in Handle<JobApi> handle) => _jobQueue[handle.Value - HandleOffset].State == JobState.Completed;

    private static void RunWorker(object obj)
    {
        var index = (int)obj;
        var thread = _threads[index];
        ref var info = ref _workers[index];
        Logger.Trace($"{thread.Name} started.", typeof(ManagedThreadPool));
        while (info.Active)
        {
            if (!_notifier.Wait(DefaultThreadWaitTime))
            {
                continue;
            }
            if (_count <= 0)
            {
                continue;
            }
            var jobIndex = GetNextQueuedJob();
            if (jobIndex == -1)
            {
                // With the way jobs are signaled, this will never happen. 
                continue;
            }

            Interlocked.Decrement(ref _count);
            ref var job = ref _jobQueue[jobIndex];
            job.Execute();
            job.State = job.JobItem.AutoReset ? JobState.Available : JobState.Completed;
        }
        Logger.Trace($"{thread.Name} stopped.", typeof(ManagedThreadPool));
    }


    public static bool Enqueue(in JobItem item, out Handle<JobApi> handle)
    {
        Unsafe.SkipInit(out handle);
        var index = GetNextJobIndex();
        if (index == -1)
        {
            return false;
        }
        ref var job = ref _jobQueue[index];
        job.JobItem = item;
        job.State = item.IsReady ? JobState.Ready : JobState.Waiting;
        Interlocked.Increment(ref _count);
        if (item.IsReady)
        {
            _notifier.Release();
        }
        handle = index + HandleOffset;
        return true;
    }


    // NOTE(Jens): the _nextJob and _nextQueuedJob are volatile fields, we can't create a single method for this unfortunately

    private static int GetNextJobIndex()
    {
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
            // NOTE(Jens): maybe we can skip this CompareExchange and just assume we got the job? The risk is that the same job can be executed twice in some cases?
            ref var job = ref _jobQueue[index];
            var previousStatus = Interlocked.CompareExchange(ref job.State, JobState.Claimed, JobState.Available);
            // If the job is busy, loop again and try to find a new spot
            if (previousStatus != JobState.Available)
            {
                continue;
            }
            return index;
        }
        Logger.Error($"Failed to get the next job ID after {_maxJobs} iterations.", typeof(ManagedThreadPool));
        return -1;
    }

    // NOTE(Jens): the _nextJob and _nextQueuedJob are volatile fields, we can't create a single method for this unfortunately

    private static int GetNextQueuedJob()
    {
        var maxIterations = _maxJobs;
        while (maxIterations-- > 0)
        {
            var current = _nextQueuedJob;
            var index = Interlocked.CompareExchange(ref _nextQueuedJob, (current + 1) % _maxJobs, current);
            // Some other thread updated the counter, do another lap
            if (index != current)
            {
                continue;
            }

            var previousStatus = Interlocked.CompareExchange(ref _jobQueue[index].State, JobState.Running, JobState.Ready);
            // If the job is busy, loop again and try to find a new spot
            if (previousStatus != JobState.Ready)
            {
                continue;
            }
            return index;
        }
        Logger.Error($"Failed to get the queued job ID after {_maxJobs} iterations.", typeof(ManagedThreadPool));
        return -1;
    }

    public static void Shutdown()
    {
        for (var i = 0; i < _workers.Length; ++i)
        {
            _workers[i].Active = false;
        }
        _notifier.Release(_threads.Length);

        foreach (var thread in _threads)
        {
            thread.Join();
        }
    }
}
