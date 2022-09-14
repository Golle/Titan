using System.Runtime.CompilerServices;
using Titan.Core.Threading2;
using Titan.ECS.Systems;

namespace Titan.ECS.Scheduler.Executors;

public struct ReversedSequentialExecutor : IExecutor
{
    [SkipLocalsInit]
    public static unsafe void RunSystems(in NodeStage stage, in JobApi jobApi)
    {
        var nodes = stage.Nodes;
        var count = stage.Count;
        for (var i = count-1; i >=0; --i)
        {
            ref readonly var node = ref nodes[i];
            var shouldRun = node.Criteria switch
            {
                RunCriteria.Always => true,
                RunCriteria.Check => node.ShouldRun(),
                RunCriteria.Once or _ => throw new NotImplementedException($"{nameof(RunCriteria)}.{nameof(RunCriteria.Once)} has not been implemented yet. Use Check or Always.")
            };
            if (shouldRun)
            {
                node.Update();
            }
        }
    }
}
