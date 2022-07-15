using System;
using System.Threading;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading2;
using Titan.ECS.SystemsV2;
using Titan.ECS.TheNew;

namespace Titan.ECS.AnotherTry;

/// <summary>
/// The scheduler is currently very simple. it has methods for Startup, Shutdown and Update. These will call different stages with different executors based on a configuration.
/// NOTE(Jens): in the future we might want a more dynamic scheduler with different stages, and maybe add "subsets" of stages.
/// NOTE(Jens): For example the EngineCoreStage.Update runs the scheduler for GameStages (PreUpdate, Update and PostUpdate).
/// </summary>
public unsafe struct Scheduler
{

    public void Update(ref JobApi jobApi, ref World world)
    {
        Thread.Sleep(500);
        Logger.Trace<Scheduler>("Update");
    }

    public void Shutdown(ref JobApi jobApi, ref World world)
    {
        Logger.Trace<Scheduler>("Shutdown");
    }

    public void Startup(ref JobApi jobApi, ref World world)
    {
        Logger.Trace<Scheduler>("Startup");
    }

    internal void Init(in ResourceCollection resources)
    {
        ref readonly var registry = ref resources.GetResource<SystemsRegistry>();


        //Logger.Trace<Scheduler>($"Init Scheduler with {systemDescriptors.Length} systems.");


    }
}
