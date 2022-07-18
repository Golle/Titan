using System;
using System.Runtime.CompilerServices;
using Titan.Core.Threading2;
using Titan.ECS.Scheduler;

namespace Titan.ECS.SystemsV2.Scheduler.Executors;

public struct SequentialExecutor : IExecutor
{
    [SkipLocalsInit]
    public static unsafe void RunSystems(in NodeStage stage, in JobApi jobApi)
    {
        if (stage.Count == 0)
        {
            return;
        }

        var nodes = stage.Nodes;
        var count = stage.Count;
        for (var i = 0; i < count; ++i)
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
