using Titan.Jobs;
using Titan.Setup;
using Titan.Systems.Scheduler;
using Titan.Windows;

namespace Titan.Runners;

public class WindowedRunner : IRunner
{
    public static IRunner Create()
        => new WindowedRunner();

    private IWindow _window;
    private SystemsScheduler _scheduler;
    private IJobApi _jobApi;

    public bool Init(IApp app)
    {
        _window = app.GetManagedResource<IWindow>();
        _scheduler = app.GetManagedResource<SystemsScheduler>();
        _jobApi = app.GetManagedResource<IJobApi>();
        return true;
    }

    public bool RunOnce()
    {
        //NOTE(Jens): temp solution to run the systems on a separate thread from the Windows message queue
        _scheduler.OnUpdate(_jobApi);
        return _window!.Update();
    }
}
