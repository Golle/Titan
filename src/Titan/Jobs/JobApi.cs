#nullable disable
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading;

namespace Titan.Jobs;

internal enum WorkerState
{
    Waiting,
    Running,
    Failed
}

internal static class JobState
{
    public const int Available = 0;
    public const int Claimed = 1;
    public const int Waiting = 2;
    public const int Ready = 3;
    public const int Running = 4;
    public const int Completed = 5;
}
internal unsafe struct Job
{
    public JobItem JobItem;
    public volatile int State; //JobState enum
    public void Execute() => JobItem.Function(JobItem.Context);
}
internal struct WorkerInfo
{
    public ThreadHandle Handle;
    public bool Active;
    public WorkerState State;
    public GCHandle JobApi;
}

internal unsafe class JobApi : IJobApi
{
    private const int HandleOffset = 1124521;
    private static readonly TimeSpan DefaultThreadWaitTime = TimeSpan.FromMilliseconds(100);
    private TitanArray<WorkerInfo> _workers;
    private TitanArray<Job> _jobQueue;
    private GCHandle _jobApiHandle;

    private volatile int _nextJob;
    private volatile int _nextQueuedJob;
    private volatile int _count;
    private int _maxJobs;

    private SemaphoreSlim _notifier;

    private IThreadManager _threadManager;
    private IMemoryManager _memoryManager;
    public bool Init(IMemoryManager memoryManager, IThreadManager threadManager, uint numberOfWorkers, uint maxQueuedJobs = 1000)
    {
        var processorCount = Environment.ProcessorCount;
        if (numberOfWorkers >= processorCount)
        {
            Logger.Warning<JobApi>($"The amount of workers is greater than the processor count. This can cause a lot of context switching and degrade performance. ProcessorCount: {processorCount} Workers:{numberOfWorkers}. Recomended is CPU count - 1(or 2)");
        }
        _workers = memoryManager.AllocArray<WorkerInfo>(numberOfWorkers, true);
        if (!_workers.IsValid)
        {
            Logger.Error<JobApi>("Failed to allocate memory for the thread handles.");
            return false;
        }

        _jobQueue = memoryManager.AllocArray<Job>(maxQueuedJobs, true);
        if (!_jobQueue.IsValid)
        {
            Logger.Error<JobApi>("Failed to allocate memory for the jobs array.");
            memoryManager.Free(ref _workers);
            return false;
        }

        _jobApiHandle = GCHandle.Alloc(this);
        _memoryManager = memoryManager;
        _threadManager = threadManager;
        _maxJobs = (int)maxQueuedJobs;
        _notifier = new SemaphoreSlim(0, _maxJobs);


        for (var i = 0; i < _workers.Length; ++i)
        {
            _workers[i].Active = true;
            _workers[i].State = WorkerState.Waiting;
            _workers[i].JobApi = _jobApiHandle;
            _workers[i].Handle = threadManager.CreateThread(new CreateThreadArgs
            {
                Callback = &RunWorker,
                Parameter = _workers.GetPointer(i),
                StartImmediately = false
            });
            threadManager.Start(_workers[i].Handle);
        }

        return true;
    }

    [UnmanagedCallersOnly]
    private static int RunWorker(void* parameter)
    {
        Debug.Assert(parameter != null);

        ref var workerInfo = ref *(WorkerInfo*)parameter;
        Debug.Assert(workerInfo.JobApi.IsAllocated);
        var jobApi = (JobApi)workerInfo.JobApi.Target;
        Debug.Assert(jobApi != null);

        while (workerInfo.Active)
        {
            if (!jobApi._notifier.Wait(DefaultThreadWaitTime))
            {
                continue;
            }

            if (jobApi._count <= 0)
            {
                continue;
            }
            var jobIndex = jobApi.GetNextQueuedJob();
            if (jobIndex == -1)
            {
                // With the way jobs are signaled, this will never happen. 
                continue;
            }
            Interlocked.Decrement(ref jobApi._count);
            ref var job = ref jobApi._jobQueue[jobIndex];
            job.Execute();
            job.State = job.JobItem.AutoReset ? JobState.Available : JobState.Completed;
        }

        return 0;
    }

    private int GetNextQueuedJob()
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

            var previousState = Interlocked.CompareExchange(ref _jobQueue[index].State, JobState.Running, JobState.Ready);
            // If the job is busy, loop again and try to find a new spot
            if (previousState != JobState.Ready)
            {
                continue;
            }
            return index;
        }
        Logger.Error<JobApi>($"Failed to get the queued job ID after {_maxJobs} iterations.");
        return -1;
    }

    public JobHandle Enqueue(in JobItem item)
    {
        var index = GetNextJobIndex();
        if (index == -1)
        {
            return JobHandle.Invalid;
        }
        ref var job = ref _jobQueue[index];
        job.JobItem = item;
        job.State = item.IsReady ? JobState.Ready : JobState.Waiting;
        Interlocked.Increment(ref _count);
        if (item.IsReady)
        {
            _notifier.Release();
        }
        return new(index + HandleOffset);
    }

    private int GetNextJobIndex()
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
            var previousState = Interlocked.CompareExchange(ref job.State, JobState.Claimed, JobState.Available);
            // If the job is busy, loop again and try to find a new spot
            if (previousState != JobState.Available)
            {
                continue;
            }
            return index;
        }
        Logger.Error<JobApi>($"Failed to get the next job ID after {_maxJobs} iterations.");
        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsCompleted(in JobHandle handle) => _jobQueue[handle.Handle - HandleOffset].State == JobState.Completed;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset(ref JobHandle handle)
    {
        Debug.Assert(handle.IsValid());
        var jobIndex = handle.Handle - HandleOffset;
        ref var job = ref _jobQueue[jobIndex];
        if (job.State == JobState.Completed)
        {
            job.State = JobState.Available;
        }
        handle = JobHandle.Invalid;
    }

    public void Shutdown()
    {
        foreach (ref var worker in _workers.AsSpan())
        {
            worker.Active = false;
        }

        foreach (ref var worker in _workers.AsSpan())
        {
            _threadManager.Join(worker.Handle);
            _threadManager.Destroy(ref worker.Handle);
            worker = default;
        }

        if (_memoryManager != null)
        {
            _memoryManager.Free(ref _workers);
            _memoryManager = null;
        }
        if (_jobApiHandle.IsAllocated)
        {
            _jobApiHandle.Free();
        }
    }
}
