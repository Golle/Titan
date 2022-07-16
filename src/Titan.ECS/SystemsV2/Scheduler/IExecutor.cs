using Titan.Core.Threading2;
using Titan.ECS.AnotherTry;

namespace Titan.ECS.SystemsV2.Scheduler;

public interface IExecutor
{
    static abstract void RunSystems(in NodeStage stage, in JobApi jobApi);
}
