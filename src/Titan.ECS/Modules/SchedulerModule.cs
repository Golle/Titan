using Titan.ECS.App;
using Titan.ECS.Scheduler;

namespace Titan.ECS.Modules;

public readonly struct SchedulerModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        var _ = builder.GetResourceOrDefault<SchedulerConfiguration>();

        return true;

    }
}
