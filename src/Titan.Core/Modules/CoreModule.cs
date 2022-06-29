namespace Titan.Core.Modules;

public readonly struct CoreModule : IModule
{
    public static void Build(IApp app)
    {
        app
            .AddModule<LoggingModule>()
            ;
    }
}
