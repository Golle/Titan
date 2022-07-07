using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS.SystemsV2;

namespace Titan.ECS.SystemsV2.Scheduler;



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

