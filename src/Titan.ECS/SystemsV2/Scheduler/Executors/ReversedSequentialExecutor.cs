using Titan.Core.Threading2;

namespace Titan.ECS.SystemsV2.Scheduler.Executors;

public struct ReversedSequentialExecutor : IExecutor
{
    public static void RunSystems(in SystemExecutionGraph graph, in JobApi _)
    {
        var nodes = graph.GetNodes();
        for (var i = nodes.Length - 1; i >= 0; --i)
        {
            if (nodes[i].ShouldRun())
            {
                nodes[i].Update();
            }
        }
    }
}
