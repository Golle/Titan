using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Threading2;
using Titan.ECS.AnotherTry;

namespace Titan.ECS.SystemsV2.Scheduler.Executors;

public struct ParallelExecutor : IExecutor
{
    [SkipLocalsInit]
    public static unsafe void RunSystems(in NodeStage stage, in JobApi jobApi)
    {
        // NOTE(Jens): this is experimental. We can call the Semaphore.Release on each job or once after all jobs have been scheduled.
        const bool signalReady = false;

        var nodes = stage.Nodes;
        var count = stage.Count;

        var systemsLeft = count;
        // NOTE(Jens): not sure if this will cause issues in the future. Maybe we should pre-allocate some buffer that we use?
        var handles = stackalloc Handle<JobApi>[count];
        var executingJobCount = 0;

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
                handles[executingJobCount++] = jobApi.Enqueue(JobItem.Create(node.Context, node.UpdateFunc, isReady: !signalReady, autoReset: false));
            }
            else
            {
                systemsLeft--;
            }
        }
#pragma warning disable CS0162
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (signalReady)
        {
            jobApi.SignalReady(new ReadOnlySpan<Handle<JobApi>>(handles, executingJobCount));
        }
#pragma warning restore CS0162

        // NOTE(Jens): need some kind of timeout for this. (maybe crash ?)
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
