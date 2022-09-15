namespace Titan.Core.Threading2;

public interface IThreadPoolApi
{
    static abstract bool Enqueue(in JobItem item, out Handle<JobApi> handle);
    static abstract void SignalReady(ReadOnlySpan<Handle<JobApi>> handles);
    static abstract bool IsCompleted(in Handle<JobApi> handle);
    static abstract void Reset(ref Handle<JobApi> handle);
    static abstract void Init(in ThreadPoolConfiguration config);
    static abstract void Shutdown();
}
