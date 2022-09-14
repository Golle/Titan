namespace Titan.Core.Threading2;

public struct ThreadPoolConfiguration : IDefault<ThreadPoolConfiguration>
{
    public uint WorkerThreads;
    public uint IOThreads;
    public uint MaxJobs;
    public static ThreadPoolConfiguration Default => new()
    {
        IOThreads = 2,
        WorkerThreads = (uint)(Environment.ProcessorCount - 1),
        MaxJobs = 200
    };
}
