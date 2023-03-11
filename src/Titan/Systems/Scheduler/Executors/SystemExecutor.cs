using Titan.Jobs;

namespace Titan.Systems.Scheduler.Executors;

internal readonly unsafe struct SystemExecutor
{
    public readonly delegate*<IJobApi, SystemNode*, int, void> Func;
    public SystemExecutor(delegate*<IJobApi, SystemNode*, int, void> executorFunc) => Func = executorFunc;
    public static SystemExecutor Create<T>() where T : unmanaged, ISystemsExecutor => new(&T.Execute);
}
