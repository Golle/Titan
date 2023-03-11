using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading;
using Titan.Core.Threading.Platform;
using Titan.Jobs;
using Titan.Setup;

namespace Titan.Modules;

internal struct ThreadingModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        IThreadManager threadManager = GlobalConfiguration.OperatingSystem switch
        {
            OperatingSystem.Windows => new ThreadManager<Win32ThreadApi>(),
            OperatingSystem.Linux => new ThreadManager<PosixThreadApi>(),
            _ => null
        };

        if (threadManager == null)
        {
            Logger.Error<ThreadingModule>($"OS {GlobalConfiguration.OperatingSystem} is not supported at the moment.");
            return false;
        }
        Logger.Trace<ThreadingModule>($"Using {threadManager.GetType().FormattedName()} for Threading");
        builder
            .AddManagedResource(threadManager)
            .AddManagedResource<IJobApi>(new JobApi());

        return true;
    }

    public static bool Init(IApp app)
    {
        var memoryManager = app.GetManagedResource<IMemoryManager>();
        var threadManager = app.GetManagedResource<IThreadManager>();
        var jobApi = (JobApi)app.GetManagedResource<IJobApi>();

        //NOTE(Jens): add config from App
        if (!jobApi.Init(memoryManager, threadManager, (uint)(Environment.ProcessorCount - 2)))
        {
            Logger.Error<ThreadingModule>("Failed to init the JobApi");
            return false;
        }
        return true;
    }

    public static bool Shutdown(IApp app)
    {
        var jobApi = (JobApi)app.GetManagedResource<IJobApi>();
        jobApi.Shutdown();

        return true;
    }
}
