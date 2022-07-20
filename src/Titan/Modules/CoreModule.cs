using Titan.Core;
using Titan.ECS.App;
using Titan.ECS.Modules;

namespace Titan.Modules;

public struct CoreModule : IModule
{
    public static void Build(AppBuilder app) =>
        app
            .AddModule<MemoryModule>()
            .AddModule<LoggingModule>()
            .AddModule<ThreadingModule>()
            .AddModule<ECSModule>()
            .AddModule<SchedulerModule>();
}
