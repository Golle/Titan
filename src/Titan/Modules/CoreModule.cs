using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Titan.Core.App;
using Titan.Core.Threading2;
using Titan.ECS.Modules;
using Titan.ECS.SystemsV2;
using Titan.ECS.SystemsV2.Scheduler;

namespace Titan.Modules;

public readonly struct CoreModule : IModule
{
    public static void Build(IApp app)
    {
        app
            .AddModule<MemoryModule>()
            .AddModule<LoggingModule>()
            .AddModule<ThreadingModule>()
            .AddModule<SchedulerModule>()
            ;
    }
}


