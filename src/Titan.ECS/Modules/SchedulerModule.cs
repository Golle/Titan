using Titan.ECS.AnotherTry;

namespace Titan.ECS.Modules;

public readonly struct SchedulerModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        var _ = builder.GetResourceOrDefault<SchedulerConfiguration>();
    }
}
