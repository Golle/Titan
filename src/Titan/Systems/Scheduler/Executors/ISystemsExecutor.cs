using Titan.Jobs;

namespace Titan.Systems.Scheduler.Executors;

internal unsafe interface ISystemsExecutor
{
    static abstract void Execute(IJobApi jobApi, SystemNode* nodes, int count);
}
