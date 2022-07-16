using System.Threading;
using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS.AnotherTry;

namespace Titan.EventNewer;

public readonly unsafe struct GameLoop
{
    private readonly Scheduler* _scheduler;
    private readonly World* _world;
    private readonly JobApi* _jobApi;
    private readonly Thread _thread;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public GameLoop(ref Scheduler scheduler, ref World world, ref JobApi jobApi)
    {
        fixed (Scheduler* pScheduler = &scheduler)
        fixed (World* pWorld = &world)
        fixed (JobApi* pJobApi = &jobApi)

        {
            _world = pWorld;
            _scheduler = pScheduler;
            _jobApi = pJobApi;
        }

        _thread = new Thread(RunSchedule)
        {
            IsBackground = true,
            Name = "GameLoop thread.",
        };
    }

    public void Start()
    {
        _thread.Start();
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _thread.Join();
    }

    private void RunSchedule(object _)
    {
        var token = _cancellationTokenSource.Token;
        Logger.Trace<Titan.GameLoop>("Starting the game loop");
        ref var scheduler = ref *_scheduler;
        ref var world = ref *_world;
        ref var jobApi = ref *_jobApi;

        while (!token.IsCancellationRequested)
        {
            scheduler.Update(ref jobApi, ref world);
        }
        Logger.Trace<Titan.GameLoop>("Ending the game loop");
    }
}
