using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS.SystemsV2;

namespace Titan.Modules;

public readonly struct ThreadingModule : IModule
{
    public static void Build(IApp app)
    {
        if (!app.HasResource<ThreadPoolConfiguration>())
        {
            app.AddResource(ThreadPoolConfiguration.Default);
        }

        ref readonly var config = ref app.GetResource<ThreadPoolConfiguration>();
        Logger.Trace<ThreadingModule>($"{nameof(ManagedThreadPool)} configuration. Worker threads: {config.WorkerThreads} MaxJobs: {config.MaxJobs} IOThreads: {config.IOThreads} (NYI)");
        
        app
            .AddResource(JobApi.CreateAndInitJobApi<ManagedThreadPool>(config))
            .AddDisposable(new DisposableAction(() =>
            {
                app.GetResource<JobApi>()
                    .Shutdown();
            }));
    }
}
