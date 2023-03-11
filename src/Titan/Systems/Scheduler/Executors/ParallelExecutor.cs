using System.Runtime.CompilerServices;
using Titan.Jobs;

namespace Titan.Systems.Scheduler.Executors;

internal struct ParallelExecutor : ISystemsExecutor
{
    [SkipLocalsInit]
    public static unsafe void Execute(IJobApi jobApi, SystemNode* nodes, int count)
    {
        if (count == 0)
        {
            return;
        }
        var jobCount = 0;
        var handles = stackalloc JobHandle[count];
        for (var i = 0; i < count; ++i)
        {
            ref readonly var node = ref nodes[i];
            var shouldRun = node.Criteria switch
            {
                RunCriteria.Always or RunCriteria.AlwaysInline => true,
                RunCriteria.Check or RunCriteria.CheckInline => node.ShouldRun(),
                RunCriteria.Once or _ => throw new NotImplementedException($"{nameof(RunCriteria)}.{node.Criteria} has not been implemented yet. Use Check, Always or AlwaysInline.")
            };
            if (shouldRun)
            {
                if (node.Criteria is RunCriteria.AlwaysInline or RunCriteria.CheckInline)
                {
                    node.UpdateFunc(node.Context);
                }
                else
                {
                    handles[jobCount++] = jobApi.Enqueue(JobItem.Create(node.Context, node.UpdateFunc, true, false));
                }
            }
        }


        while (true)
        {
            var completed = true;
            for (var i = 0; i < jobCount; ++i)
            {
                if (!jobApi.IsCompleted(handles[i]))
                {
                    completed = false;
                    break;
                }
            }
            if (completed)
            {
                break;
            }
        }

        for (var i = 0; i < jobCount; ++i)
        {
            jobApi.Reset(ref handles[i]);
        }

    }
}
