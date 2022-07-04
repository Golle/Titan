using Titan.ECS.SystemsV2;

namespace Titan.Modules;

public readonly struct CoreModule : IModule
{
    public static void Build(IApp app)
    {
        app
            .AddModule<LoggingModule>()
            ;
    }
}
