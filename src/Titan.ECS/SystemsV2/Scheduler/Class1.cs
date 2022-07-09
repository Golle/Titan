using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Core.Threading2;
using static Titan.ECS.SystemsV2.Scheduler.SchedulerTest;

namespace Titan.ECS.SystemsV2.Scheduler;


struct World
{
    public TransientMemory Allocator;

    public SystemExecutionGraph Graph;

}

internal struct SynchronousExecutor : IExecutor
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

internal struct ParallelExecutor : IExecutor
{
    [SkipLocalsInit]
    public static unsafe void RunSystems(in SystemExecutionGraph graph, in JobApi jobApi)
    {
        var nodes = graph.GetNodes();
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
                handles[executingJobCount++] = jobApi.Enqueue(JobItem.Create(node.System.Instance, node.System.Update, isReady: false, autoReset: false));
            }
        }

        jobApi.SignalReady(handles[..executingJobCount]);
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

public unsafe struct OrderedExecutor : IExecutor
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    [SkipLocalsInit]
    public static void RunSystems(in SystemExecutionGraph graph, in JobApi jobApi)
    {
        var nodes = graph.GetNodes();
        var nodeCount = nodes.Length;
        var systemsLeft = nodeCount;

        var states = stackalloc SystemState[nodeCount];
        var handles = stackalloc Handle<JobApi>[nodeCount];

        // Check for systems that should run and set the state and handle. (Start systems with no dependencies)
        for (var index = 0; index < nodeCount; index++)
        {
            ref readonly var node = ref nodes[index];
            if (!node.ShouldRun())
            {
                systemsLeft--;
                states[index] = SystemState.Completed;
                handles[index] = Handle<JobApi>.Null;
            }
            else
            {
                if (node.DependenciesCount == 0)
                {
                    ref readonly var system = ref node.System;
                    handles[index] = jobApi.Enqueue(JobItem.Create(system.Instance, system.Update, autoReset: false));
                    states[index] = SystemState.Running;
                    //Logger.Trace<OrderedExecutor>($"System with index {index} started");
                }
                else
                {
                    states[index] = SystemState.Waiting;
                    handles[index] = Handle<JobApi>.Null;
                }
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
                    handles[i] = RunSystem(jobApi, nodes[i].System);
                    states[i] = SystemState.Running;
                    //Logger.Trace<OrderedExecutor>($"System with index {i} started");
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsReady(in SystemExecutionGraphNode node, SystemState* states)
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
        static Handle<JobApi> RunSystem(in JobApi jobApi, in SystemNode system)
            => jobApi.Enqueue(JobItem.Create(system.Instance, system.Update, autoReset: false));

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

public interface IExecutor
{
    static abstract void RunSystems(in SystemExecutionGraph graph, in JobApi jobApi);
}




public class Scheduler // this is the graph
{
    public SystemExecutionGraph Graph;

    public void Test()
    {

    }
}



public static unsafe class SchedulerTest
{

    // get all systems with their dependencies
    public enum SystemState
    {
        Waiting,
        Running,
        Completed
    }

    public struct SystemT
    {
        public SystemState State;
        public Handle<JobApi> Handle;
    }

    //[SkipLocalsInit]
    //public static void Func(Span<ASystemNode> systems, in JobApi jobApi)
    //{
    //    var systemCount = systems.Length;
    //    var systemsLeft = systemCount;
    //    var state = stackalloc SystemFjopp[systems.Length];

    //    //// Check for any system that should not be scheduled
    //    for (var i = 0; i < systems.Length; ++i)
    //    {
    //        ref var system = ref systems[i];
    //        if (system.ShouldRun())
    //        {
    //            Logger.Error($"Set system {system.Id} to waiting.", typeof(SchedulerTest));
    //            state[system.Id].State = SystemState.Waiting;
    //            state[system.Id].Handle = Handle<JobApi>.Null;
    //        }
    //        else
    //        {
    //            Logger.Error($"Set system {system.Id} to completed.", typeof(SchedulerTest));
    //            state[system.Id].State = SystemState.Completed;
    //            systemsLeft--;
    //        }
    //    }

    //    while (systemsLeft > 0)
    //    {
    //        for (var i = 0; i < systems.Length; ++i)
    //        {
    //            ref var system = ref systems[i];
    //            if (IsReadyToRun(system))
    //            {
    //                Logger.Error($"Starting system {system.Id}");
    //                state[system.Id].State = SystemState.Running;
    //                uint a;
    //                state[system.Id].Handle = jobApi.Enqueue(JobItem.Create(ref a, system.Update))
    //            }
    //        }

    //        foreach (ref var system in systems)
    //        {
    //            if (state[system.Id] == SystemState.Running && system.IterationsLeft > 0)
    //            {
    //                system.IterationsLeft--;
    //                if (system.IterationsLeft == 0)
    //                {

    //                    state[system.Id] = SystemState.Completed;
    //                    systemsLeft--;
    //                    Logger.Error($"Completed system {system.Id} on iteration {debugIterations}. {systemsLeft} systems left.");
    //                }
    //            }
    //        }

    //        Thread.Sleep(400);
    //    }


    //bool IsReadyToRun(in ASystemNode node)
    //{
    //    if (state[node.Id].State != SystemState.Waiting)
    //        return false;

    //    for (var i = 0; i < node.DependenciesCount; ++i)
    //    {
    //        if (state[node.Dependencies[i]].State != SystemState.Completed)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}
    //}
}

