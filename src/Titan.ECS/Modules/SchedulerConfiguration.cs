using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.App;
using Titan.ECS.SystemsV2;
using Titan.ECS.SystemsV2.Scheduler;
using Titan.ECS.SystemsV2.Scheduler.Executors;

namespace Titan.ECS.Modules;

[StructLayout(LayoutKind.Explicit, Size = sizeof(long) * (int)Stage.Count)]
public unsafe struct SchedulerConfiguration : IDefault<SchedulerConfiguration>
{
    public static SchedulerConfiguration Default()
    {
        var config = new SchedulerConfiguration();
        config.SetExecutor<SequentialExecutor>(Stage.PreStartup);
        config.SetExecutor<SequentialExecutor>(Stage.Startup);
        config.SetExecutor<ParallelExecutor>(Stage.PreUpdate);
        config.SetExecutor<OrderedExecutor>(Stage.Update);
        config.SetExecutor<ParallelExecutor>(Stage.PostUpdate);
        config.SetExecutor<SequentialExecutor>(Stage.Shutdown);
        config.SetExecutor<SequentialExecutor>(Stage.PostShutdown);
        return config;
    }

    public void SetExecutor<T>(Stage stage) where T : IExecutor
    {
        Debug.Assert(stage != Stage.Count);
        var pData = (StageExecutor*)Unsafe.AsPointer(ref this);
        pData[(int)stage] = new StageExecutor { RunFunc = &T.RunSystems };
    }

    public ReadOnlySpan<StageExecutor> Get() => new(Unsafe.AsPointer(ref this), (int)Stage.Count);
}
