using Titan.Core;

namespace Titan.ECS.Modules;

public struct ECSConfiguration : IDefault<ECSConfiguration>
{
    public const uint DefaultMaxEntities = 10_000;
    public const uint DefaultMaxEventTypes = 200;
    public const uint DefaultEventStreamSize = 2 * 1024 * 1024; // 2MB
    public uint MaxEntities;
    public uint EventStreamSize;
    public uint MaxEventTypes;

    public ECSConfiguration()
    {
        MaxEntities = DefaultMaxEntities;
        EventStreamSize = DefaultEventStreamSize;
        MaxEventTypes = DefaultMaxEventTypes;
    }

    public static ECSConfiguration Default => new();
}
