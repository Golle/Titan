using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.SystemsV2;

namespace Titan.ECS.Modules;

public struct ECSModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        ref readonly var config = ref builder.GetResourceOrDefault<ECSConfiguration>();

        Logger.Trace<ECSModule>($"MaxEntities: {config.MaxEntities}");
        // Create entity manager?

        // These numbers probably needs tweaking.
        builder
            .AddEvent<EntityDestroyed>(1000)
            .AddEvent<ComponentDestroyed>(1000);
    }
}
