using Titan.ECS.App;
using Titan.ECS.Modules;
using Titan.FileSystem;

namespace Titan.Modules;

public struct CoreModule : IModule
{
    public static bool Build(AppBuilder app)
    {
        app
            .AddModule<MemoryModule>()
            .AddModule<FileSystemModule>()
            .AddModule<LoggingModule>()
            .AddModule<ThreadingModule>()
            .AddModule<ECSModule>()
            .AddModule<SchedulerModule>();
        return true;
    }
}
