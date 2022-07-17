using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS;
using Titan.ECS.App;
using Titan.ECS.SystemsV2;

namespace Titan.Modules;
public struct ThreadingModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        ref readonly var config = ref builder.GetResourceOrDefault<ThreadPoolConfiguration>();

        Logger.Trace<ThreadingModule>($"{nameof(ManagedThreadPool)} configuration. Worker threads: {config.WorkerThreads} MaxJobs: {config.MaxJobs} IOThreads: {config.IOThreads} (NYI)");

        builder
            .AddResource(JobApi.CreateAndInitJobApi<ManagedThreadPool>(config))
            .AddSystemToStage<JobApiTeardown>(Stage.PostShutdown);
    }

    private struct JobApiTeardown : IStructSystem<JobApiTeardown>
    {
        private ApiResource<JobApi> _jobApi;
        public static void Init(ref JobApiTeardown system, in SystemsInitializer init) => system._jobApi = init.GetApi<JobApi>();
        public static void Update(ref JobApiTeardown system) => system._jobApi.Get().Shutdown();
        public static bool ShouldRun(in JobApiTeardown _) => true;
    }
}
