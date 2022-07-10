using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.SystemsV2;

namespace Titan.Modules;
public struct MemoryDescriptor : IDefault<MemoryDescriptor>
{
    private const uint DefaultPermanentMemory = 256 * 1024 * 1024;
    private const uint DefaultTransientMemory = 32 * 1024 * 1024;

    public uint TransientMemory;
    public uint PermanentMemory;
    public static MemoryDescriptor Default() =>
        new()
        {
            PermanentMemory = DefaultPermanentMemory,
            TransientMemory = DefaultTransientMemory
        };
}
public readonly struct MemoryModule : IModule
{
    public static void Build(IApp app)
    {
        ref readonly var descriptor = ref app.GetResourceOrDefault<MemoryDescriptor>();
        Logger.Trace<MemoryModule>($"Perment memory: {descriptor.PermanentMemory} bytes. Transient memory: {descriptor.TransientMemory}");
        var memoryPool = app.GetResource<MemoryPool>();
        app.AddResource(memoryPool.CreateAllocator<PermanentMemory>(descriptor.PermanentMemory))
            .AddResource(memoryPool.CreateAllocator<TransientMemory>(descriptor.TransientMemory));
    }
}
