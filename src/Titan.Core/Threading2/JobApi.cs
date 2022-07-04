namespace Titan.Core.Threading2;

public readonly unsafe struct JobApi
{
    private readonly delegate*<in JobItem, out Handle<JobApi>, bool> _enqueue;
    private readonly delegate*<void> _shutdown;
    public JobApi(delegate*<in JobItem, out Handle<JobApi>, bool> enqueue, delegate*<void> shutdown)
    {
        _enqueue = enqueue;
        _shutdown = shutdown;
    }

    public bool IsCompleted(in Handle<JobApi> handle)
    {
        return true;
    }

    public Handle<JobApi> Enqueue(in JobItem jobItem) => _enqueue(jobItem, out var handle) ? handle : Handle<JobApi>.Null;
    public void Shutdown() => _shutdown();
    public static JobApi CreateAndInitJobApi<T>(in ThreadPoolConfiguration config) where T : IThreadPoolApi
    {
        T.Init(config);
        return new JobApi(
            &T.Enqueue,
            &T.Shutdown
        );
    }
}
