using Titan.ECS.SystemsV2;

namespace Titan.Modules;

public readonly struct CoreModule : IModule
{
    public static void Build(IApp app)
    {

        app
            .AddModule<MemoryModule>()
            .AddModule<LoggingModule>()
            .AddModule<ThreadingModule>()
            ;
    }
}

public readonly struct MemoryModule : IModule
{
    public static void Build(IApp app)
    {
        if (!app.HasResource<MemoryDescriptor>())
        {

        }


    }
}

public struct MemoryDescriptor
{
    public uint TransientMemory;
    public uint PermanentMemory;


}
