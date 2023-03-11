using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Modules;
using Titan.Setup;
using Titan.Systems;

namespace Titan.Memory;

internal unsafe struct MemoryModule : IModule
{
    // 8MB of temporary frame memory. This should be configurable
    private static readonly uint TempMemorySize = MemoryUtils.MegaBytes(8);
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddResource(new PerFrameArena())
            // Run this system first and inline
            .AddSystemToStage<TemporaryMemorySystem>(SystemStage.First, RunCriteria.AlwaysInline, int.MaxValue)
            ;

        return true;
    }

    public static bool Init(IApp app)
    {
        var memoryManager = app.GetManagedResource<IMemoryManager>();
        ref var allocator = ref app.GetResource<PerFrameArena>();
        var mem = memoryManager.Alloc(TempMemorySize);
        if (mem == null)
        {
            Logger.Error<MemoryModule>($"Failed to allocate {TempMemorySize} bytes of memory for the PerFrameMemory allocator");
            return false;
        }
        allocator = new PerFrameArena(mem, TempMemorySize);
        return true;
    }

    public static bool Shutdown(IApp app)
    {
        var memoryManager = app.GetManagedResource<IMemoryManager>();
        ref var allocator = ref app.GetResource<PerFrameArena>();
        memoryManager.Free(allocator.GetPointer());
        allocator = default;

        return true;
    }
}
