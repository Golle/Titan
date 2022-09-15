using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Threading2;
using Titan.ECS.Systems;

namespace Titan.ECS.Scheduler.Executors;

public unsafe struct OrderedExecutor : IExecutor
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    [SkipLocalsInit]
    public static void RunSystems(in NodeStage stage, in JobApi jobApi)
    {
        if (stage.Count == 0)
        {
            return;
        }

        var nodes = stage.Nodes;
        var nodeCount = stage.Count;

        var systemsLeft = nodeCount;

        var states = stackalloc SystemState[nodeCount];
        var handles = stackalloc Handle<JobApi>[nodeCount];

        // Check for systems that should run and set the state and handle. (Start systems with no dependencies)
        for (var index = 0; index < nodeCount; index++)
        {
            ref readonly var node = ref nodes[index];
            var shouldRun = node.Criteria switch
            {
                RunCriteria.Always => true,
                RunCriteria.Check => node.ShouldRun(),
                RunCriteria.Once or _ => throw new NotImplementedException($"{nameof(RunCriteria)}.{nameof(RunCriteria.Once)} has not been implemented yet. Use Check or Always.")
            };

            if (shouldRun)
            {
                if (node.DependenciesCount == 0)
                {
                    handles[index] = RunSystem(jobApi, node);
                    states[index] = SystemState.Running;
                    //Logger.Trace<OrderedExecutor>($"System with index {index} started");
                }
                else
                {
                    states[index] = SystemState.Waiting;
                    handles[index] = Handle<JobApi>.Null;
                }
            }
            else
            {
                systemsLeft--;
                states[index] = SystemState.Completed;
                handles[index] = Handle<JobApi>.Null;
            }
        }

        while (systemsLeft > 0)
        {
            UpdateJobStates(jobApi, handles, states, nodeCount, ref systemsLeft);

            for (var i = 0; i < nodeCount; ++i)
            {
                if (states[i] != SystemState.Waiting)
                {
                    continue;
                }

                if (IsReady(nodes[i], states))
                {
                    handles[i] = RunSystem(jobApi, nodes[i]);
                    states[i] = SystemState.Running;
                    //Logger.Trace<OrderedExecutor>($"System with index {i} started");
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsReady(in Node node, SystemState* states)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Handle<JobApi> RunSystem(in JobApi jobApi, in Node system)
            => jobApi.Enqueue(JobItem.Create(system.Context, system.UpdateFunc, autoReset: false));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void UpdateJobStates(in JobApi jobApi, in Handle<JobApi>* handles, in SystemState* states, int nodeCount, ref int systemsLeft)
        {
            for (var i = 0; i < nodeCount; ++i)
            {
                // Check if the job is completed, if it is reset the job and update the state
                if (handles[i].IsValid() && jobApi.IsCompleted(handles[i]))
                {
                    jobApi.Reset(ref handles[i]);
                    states[i] = SystemState.Completed;
                    systemsLeft--;
                    //Logger.Trace<OrderedExecutor>($"System with index {i} completed. {systemsLeft} left.");
                }
            }
        }

    }

}
