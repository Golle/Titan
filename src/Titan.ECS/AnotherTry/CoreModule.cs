using System;
using Titan.Core;
using Titan.Core.Logging;

namespace Titan.ECS.AnotherTry;

public struct CoreModule : IModule2
{
    public static void Build(AppBuilder app)
    {
        app
            .AddModule<LoggingModule>()
            .AddModule<ThreadingModule>()
            .AddModule<ECSModule>()
            ;

    }
}

public struct EntityConfiguration : IDefault<EntityConfiguration>
{
    private const uint DefaultMaxEntities = 10_000;
    public uint MaxEntities;

    public static EntityConfiguration Default
        => new()
        {
            MaxEntities = DefaultMaxEntities
        };
}

public struct ECSModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        ref readonly var config = ref builder.GetResourceOrDefault<EntityConfiguration>();
        
        Logger.Trace<ECSModule>($"MaxEntities: {config.MaxEntities}");
        // Create entity manager?
    }
}
