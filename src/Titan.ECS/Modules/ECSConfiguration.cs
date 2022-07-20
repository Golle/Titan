using Titan.Core;

namespace Titan.ECS.Modules;

public struct ECSConfiguration : IDefault<ECSConfiguration>
{
    private const uint DefaultMaxEntities = 10_000;
    public uint MaxEntities;

    public static ECSConfiguration Default
        => new()
        {
            MaxEntities = DefaultMaxEntities
        };
}
