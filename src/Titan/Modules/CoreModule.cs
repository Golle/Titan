using Titan.Assets;
using Titan.BuiltIn;
using Titan.ECS;
using Titan.Memory;
using Titan.Setup;

namespace Titan.Modules;

public struct CoreModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddModule<EventsModule>()
            .AddModule<IOModule>()
            .AddModule<ThreadingModule>()
            .AddModule<ECSModule>()
            .AddModule<SchedulerModule>()
            .AddModule<MemoryModule>()
            .AddModule<AssetsModule>()
#if DEBUG
            .AddModule<Editor.EditorModule>()
#endif
            .AddModule<BuiltInModule>()
            
            ;

        return true;
    }
    public static bool Init(IApp app) => true;
    public static bool Shutdown(IApp app) => true;
}
