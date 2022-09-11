using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS.Worlds;

namespace Titan.ECS.Scheduler;

public struct HeadlessRunner : IRunner
{
    // NOTE(Jens): just a sample, might be able to use something like this for a server.
    public static void Run(ref ECS.Scheduler.Scheduler scheduler, ref World world)
    {
        ref var jobApi = ref world.GetApi<JobApi>();

        Logger.Warning<HeadlessRunner>("There's no way to exit this runner yet. Just for experimenting.");
        scheduler.Startup(ref jobApi, ref world);
        while (true)
        {
            scheduler.Update(ref jobApi, ref world);
        }
        scheduler.Shutdown(ref jobApi, ref world);
    }
}
