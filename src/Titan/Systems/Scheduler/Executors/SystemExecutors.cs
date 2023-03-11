using Titan.Core.Logging;

namespace Titan.Systems.Scheduler.Executors;

internal class SystemExecutors
{
    private readonly SystemExecutor[] _executors = new SystemExecutor[(int)SystemStage.Count];
    public void Set<T>(SystemStage stage) where T : unmanaged, ISystemsExecutor
    {
        Logger.Trace<SystemExecutors>($"Using {typeof(T).Name} for System Stage {stage}");
        _executors[(int)stage] = SystemExecutor.Create<T>();
    }

    public SystemExecutor GetExecutor(SystemStage stage) => _executors[(int)stage];
}
