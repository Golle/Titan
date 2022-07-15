using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS.AnotherTry;
using Titan.Graphics.Modules;

namespace Titan.EventNewer;

public struct WindowRunner : IRunner
{
    public static void Run(ref Scheduler scheduler, ref World world)
    {
        if (!CheckPrerequisites(ref world))
        {
            return;
        }

        ref readonly var window = ref world.GetResource<Window>();
        ref var windowApi = ref world.GetApi<WindowApi>();
        ref var jobApi = ref world.GetApi<JobApi>();

        scheduler.Startup(ref jobApi, ref world);
        var gameLoop = new GameLoop(ref scheduler, ref world, ref jobApi);
        gameLoop.Start();
        // Setup everything!
        while (windowApi.Update(window))
        {
            // Noop
        }

        gameLoop.Stop();
        // Teardown everything!
        scheduler.Shutdown(ref jobApi, ref world);
    }

    private static bool CheckPrerequisites(ref World world)
    {
        if (!world.HasResource<Window>())
        {
            Logger.Error<WindowRunner>($"Resource {nameof(Window)} is missing. Please add the {nameof(WindowModule)}.");
            return false;
        }

        if (!world.HasResource<WindowApi>())
        {
            Logger.Error<WindowRunner>($"Resource {nameof(WindowApi)} is missing. Please add the {nameof(WindowModule)}.");
            return false;
        }

        if (!world.HasResource<JobApi>())
        {
            Logger.Error<WindowRunner>($"Resource {nameof(JobApi)} is missing. Please add the {nameof(ThreadingModule)}.");
            return false;
        }

        return true;
    }
}
