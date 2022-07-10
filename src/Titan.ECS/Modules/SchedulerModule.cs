using Titan.Core.Logging;
using Titan.ECS.SystemsV2;

namespace Titan.ECS.Modules;

public readonly struct SchedulerModule : IModule
{
    public static void Build(IApp app)
    {
        var _ = app.GetResourceOrDefault<SchedulerConfiguration>();

        Logger.Warning<SchedulerModule>("Not using the scheduler config for anything yet");
    }
}