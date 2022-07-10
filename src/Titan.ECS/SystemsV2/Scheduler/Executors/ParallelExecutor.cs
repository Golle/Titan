using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Threading2;

namespace Titan.ECS.SystemsV2.Scheduler.Executors;

public struct ParallelExecutor : IExecutor
{
    [SkipLocalsInit]
    public static unsafe void RunSystems(in SystemExecutionGraph graph, in JobApi jobApi)
    {
        var nodes = graph.GetNodes().Slice(0, 1);

        //var s = nodes[0].System.ReourceId.ToString();
        var length = nodes.Length;
        var systemsLeft = length;
        var executingJobCount = 0;
        Span<Handle<JobApi>> handles = stackalloc Handle<JobApi>[nodes.Length];
        for (var i = 0; i < length; ++i)
        {
            ref readonly var node = ref nodes[i];
            if (!node.ShouldRun())
            {
                systemsLeft--;
            }
            else
            {
                //Logger.Trace<ParallelExecutor>($"Start system with index {i}");
                handles[executingJobCount++] = jobApi.Enqueue(JobItem.Create(node.System.Instance, node.System.Update, isReady: true, autoReset: false));
            }
        }

        //jobApi.SignalReady(handles[..executingJobCount]);
        while (systemsLeft > 0)
        {
            for (var i = 0; i < executingJobCount; ++i)
            {
                ref var handle = ref handles[i];
                if (handle.IsValid())
                {
                    if (jobApi.IsCompleted(handle))
                    {
                        jobApi.Reset(ref handle);
                        systemsLeft--;
                        //Logger.Trace<ParallelExecutor>($"System with index {i} finished, {systemsLeft} systems left.");
                    }
                }
            }
            Thread.Yield();
        }
    }
}
