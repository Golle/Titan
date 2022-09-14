namespace Titan.Core.Threading2;

public readonly unsafe struct JobApi : IApi
{
    private readonly delegate*<in JobItem, out Handle<JobApi>, bool> _enqueue;
    private readonly delegate*<void> _shutdown;
    private readonly delegate*<in Handle<JobApi>, bool> _isCompleted;
    private readonly delegate*<ref Handle<JobApi>, void> _reset;
    private readonly delegate*<ReadOnlySpan<Handle<JobApi>>, void> _signalReady;

    public JobApi(
        delegate*<in JobItem, out Handle<JobApi>, bool> enqueue, 
        delegate*<void> shutdown,
        delegate*<in Handle<JobApi>, bool> isCompleted,
        delegate*<ref Handle<JobApi>, void> reset,
        delegate*<ReadOnlySpan<Handle<JobApi>>, void> signalReady)
    {
        _enqueue = enqueue;
        _shutdown = shutdown;
        _isCompleted = isCompleted;
        _reset = reset;
        _signalReady = signalReady;
    }

    public bool IsCompleted(in Handle<JobApi> handle) => _isCompleted(handle);
    public void Reset(ref Handle<JobApi> handle) => _reset(ref handle);
    public void SignalReady(ReadOnlySpan<Handle<JobApi>> handles) => _signalReady(handles);

    public Handle<JobApi> Enqueue(in JobItem jobItem) => _enqueue(jobItem, out var handle) ? handle : Handle<JobApi>.Null;
    public void Shutdown() => _shutdown();
    public static JobApi CreateAndInitJobApi<T>(in ThreadPoolConfiguration config) where T : IThreadPoolApi
    {
        T.Init(config);
        return new JobApi(
            &T.Enqueue,
            &T.Shutdown,
            &T.IsCompleted,
            &T.Reset,
            &T.SignalReady
        );
    }
}
