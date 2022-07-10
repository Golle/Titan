using Titan.Core.Threading2;

namespace Titan.ECS.SystemsV2.Scheduler;

public interface IExecutor
{
    static abstract void RunSystems(in SystemExecutionGraph graph, in JobApi jobApi);
}
