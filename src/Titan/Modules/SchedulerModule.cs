using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Resources;
using Titan.Setup;
using Titan.Systems;
using Titan.Systems.Scheduler;
using Titan.Systems.Scheduler.Executors;

namespace Titan.Modules;

internal struct SchedulerModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddManagedResource(new SystemsScheduler())

            ;
        return true;
    }

    public static bool Init(IApp app)
    {
        var memorymanager = app.GetManagedResource<IMemoryManager>();

        var scheduler = app.GetManagedResource<SystemsScheduler>();
        var registry = app.GetManagedResource<SystemsRegistry>();
        var resourceCollection = app.GetManagedResource<ResourceCollection>();

        var descriptors = registry.GetDescriptors();
        Logger.Trace<SchedulerModule>($"Found {descriptors.Length} system descriptors.");
        var executors = GetDefaultExecutors();
        if (!scheduler.Init(memorymanager, descriptors, executors, resourceCollection))
        {
            Logger.Error<SchedulerModule>("Failed to init the Systems.");
            return false;
        }

        return true;
    }

    private static SystemExecutors GetDefaultExecutors()
    {
        //NOTE(Jens): move this to configuration later
        var executors = new SystemExecutors();
        executors.Set<SequentialExecutor>(SystemStage.First);
        executors.Set<SequentialExecutor>(SystemStage.PreUpdate);
        executors.Set<OrderedExecutor>(SystemStage.Update);
        executors.Set<ReversedSequentialExecutor>(SystemStage.PostUpdate);
        executors.Set<ReversedSequentialExecutor>(SystemStage.Last);
        return executors;
    }

    public static bool Shutdown(IApp app)
    {
        //NOTE(Jens): add shutdown logic here, cleanup etc
        var scheduler = app.GetManagedResource<SystemsScheduler>();
        scheduler.Shutdown();
        return true;
    }
}
