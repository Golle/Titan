using Titan.Core.Memory;
using Titan.ECS.TheNew;

namespace Titan.ECS.SystemsV2;

public record CreateWorldArgs(WorldConfig Config)
{
    public SystemDescriptorCollection GlobalSystems { get; init; }
}

public struct WorldCollection
{
    // NOTE(Jens): we could have a world collection that we can add worlds to, and access them with a name (or id).
}

public struct TheWorld
{
    public readonly uint MaxEntities;
    private UnmanagedResources _resources;

    private TheWorld(uint maxEntities, UnmanagedResources resources)
    {
        MaxEntities = maxEntities;

    }

    //public static TheWorld CreateWorld(IMemoryAllocator allocator, CreateWorldArgs args)
    //{
    //    var resources = new UnmanagedResources(32 * 1024 * 1024, 200, allocator);

    //    return new TheWorld(args.Config.MaxEntities, resources);
    //}
}
