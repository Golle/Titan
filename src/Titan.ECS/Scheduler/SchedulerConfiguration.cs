using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.ECS.Scheduler.Executors;

namespace Titan.ECS.Scheduler;

[StructLayout(LayoutKind.Explicit, Size = sizeof(long) * (int)Stage.Count)]
public unsafe struct SchedulerConfiguration : IDefault<SchedulerConfiguration>
{
    public static SchedulerConfiguration Default
    {
        get
        {
            var config = new SchedulerConfiguration();
            config.SetExecutor<SequentialExecutor>(Stage.PreStartup);
            config.SetExecutor<SequentialExecutor>(Stage.Startup);
            config.SetExecutor<SequentialExecutor>(Stage.First);
            config.SetExecutor<OrderedExecutor>(Stage.PreUpdate);
            config.SetExecutor<OrderedExecutor>(Stage.Update);
            config.SetExecutor<OrderedExecutor>(Stage.PostUpdate);
            config.SetExecutor<ReversedSequentialExecutor>(Stage.Shutdown);
            config.SetExecutor<ReversedSequentialExecutor>(Stage.PostShutdown);
            return config;
        }
    }

    public static SchedulerConfiguration SingleThreaded
    {
        get
        {
            var config = new SchedulerConfiguration();
            config.SetExecutor<SequentialExecutor>(Stage.PreStartup);
            config.SetExecutor<SequentialExecutor>(Stage.Startup);
            config.SetExecutor<SequentialExecutor>(Stage.First);
            config.SetExecutor<SequentialExecutor>(Stage.PreUpdate);
            config.SetExecutor<SequentialExecutor>(Stage.Update);
            config.SetExecutor<SequentialExecutor>(Stage.PostUpdate);
            config.SetExecutor<ReversedSequentialExecutor>(Stage.Shutdown);
            config.SetExecutor<ReversedSequentialExecutor>(Stage.PostShutdown);
            return config;
        }
    }

    public void SetExecutor<T>(Stage stage) where T : IExecutor
    {
        Debug.Assert(stage != Stage.Count);
        var pData = (StageExecutor*)Unsafe.AsPointer(ref this);
        pData[(int)stage] = new StageExecutor { RunFunc = &T.RunSystems };
    }

    public readonly ReadOnlySpan<StageExecutor> Get()
    {
        fixed (void* ptr = &this)
        {
            return new(ptr, (int)Stage.Count);
        }
    }
}
