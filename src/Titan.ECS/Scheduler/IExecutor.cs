using Titan.Core.Threading2;

namespace Titan.ECS.Scheduler;

public interface IExecutor
{
    static abstract void RunSystems(in NodeStage stage, in JobApi jobApi);
}
