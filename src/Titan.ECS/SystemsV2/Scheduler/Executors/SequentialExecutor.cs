using Titan.Core.Threading2;

namespace Titan.ECS.SystemsV2.Scheduler.Executors;

public struct SequentialExecutor : IExecutor
{
    public static void RunSystems(in SystemExecutionGraph graph, in JobApi _)
    {
        foreach (ref readonly var node in graph.GetNodes())
        {
            if (node.ShouldRun())
            {
                node.Update();
            }
        }
    }
}