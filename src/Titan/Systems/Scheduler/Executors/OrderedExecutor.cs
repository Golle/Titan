using System.Runtime.CompilerServices;
using Titan.Jobs;

namespace Titan.Systems.Scheduler.Executors;

file enum SystemState
{
    Waiting,
    Running,
    Completed
}

internal readonly struct OrderedExecutor : ISystemsExecutor
{
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Execute(IJobApi jobApi, SystemNode* nodes, int count)
    {
        if (count == 0)
        {
            return;
        }

        var states = stackalloc SystemState[count];
        var handles = stackalloc JobHandle[count];
        var systemsLeft = count;

        // The initial setup and execute some systems
        for (var index = 0; index < count; ++index)
        {
            ref var node = ref nodes[index];
            var shouldRun = node.Criteria switch
            {
                RunCriteria.Always or RunCriteria.AlwaysInline => true,
                RunCriteria.Check or RunCriteria.CheckInline => node.ShouldRun(),
                RunCriteria.Once or _ => throw new NotImplementedException($"The {node.Criteria} has not been implemented yet."),
            };

            if (shouldRun)
            {
                // the system have no dependencies, just start it.
                if (node.DependenciesCount == 0)
                {
                    // Inline systems, just run them
                    if (node.Criteria is RunCriteria.CheckInline or RunCriteria.AlwaysInline)
                    {
                        node.Update();
                        states[index] = SystemState.Completed;
                        handles[index] = JobHandle.Invalid;
                        systemsLeft--;
                    }
                    else
                    {
                        handles[index] = jobApi.Enqueue(JobItem.Create(node.Context, node.UpdateFunc, true, false));
                        states[index] = SystemState.Running;
                    }
                }
                else
                {
                    // the system has dependencies, put it in waiting stage (we should check the dependencies here, but might be unnecessary)
                    states[index] = SystemState.Waiting;
                    handles[index] = JobHandle.Invalid;
                }
            }
            else
            {
                // the system is marked as should not run, set it to completed and decrement the counter
                systemsLeft--;
                states[index] = SystemState.Completed;
                handles[index] = JobHandle.Invalid;
            }
        }

        while (systemsLeft > 0)
        {
            for (var index = 0; index < count; ++index)
            {

                // update the job handle
                ref var jobHandle = ref handles[index];
                if (jobHandle.IsValid() && jobApi.IsCompleted(jobHandle))
                {
                    jobApi.Reset(ref jobHandle);
                    states[index] = SystemState.Completed;
                    systemsLeft--;
                }

                // if the system is not in a Waiting state, move to next.
                if (states[index] != SystemState.Waiting)
                {
                    continue;
                }

                ref var node = ref nodes[index];
                if (!IsReady(node, states))
                {
                    continue;
                }

                if (node.Criteria is RunCriteria.CheckInline or RunCriteria.AlwaysInline)
                {
                    node.Update();
                    states[index] = SystemState.Completed;
                    systemsLeft--;
                }
                else
                {
                    handles[index] = jobApi.Enqueue(JobItem.Create(node.Context, node.UpdateFunc, true, false));
                    states[index] = SystemState.Running;
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsReady(in SystemNode node, SystemState* states)
        {
            for (var i = 0; i < node.DependenciesCount; ++i)
            {
                if (states[node.Dependencies[i]] != SystemState.Completed)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
