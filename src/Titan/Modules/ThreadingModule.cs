using Titan.Core.App;
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
            .AddSystemToStage<JobApiTeardown>(Stage.PostShutdown);
    }
    // NOTE(Jens): this should not be a system, add this to App instead. 
    private struct JobApiTeardown : IStructSystem<JobApiTeardown>
    {
        private ApiResource<JobApi> _jobApi;
        public static void Init(ref JobApiTeardown system, in SystemsInitializer init) => system._jobApi = init.GetApi<JobApi>();
        public static void Update(ref JobApiTeardown system)
        {
            Logger.Trace<JobApiTeardown>("Shutting down job api");
            system._jobApi.Get().Shutdown();
        }

        public static bool ShouldRun(in JobApiTeardown _) => true;
    }
}
