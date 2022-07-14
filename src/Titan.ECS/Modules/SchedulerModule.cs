using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.App;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading2;
using Titan.ECS.SystemsV2;
using Titan.ECS.SystemsV2.Scheduler;

namespace Titan.ECS.Modules;

public unsafe struct GameLoopThread : IRunner
{
    private static Thread _thread;
    private static delegate*<Runner*, void> _update;
    private static delegate*<Runner*, void> _start;
    private static delegate*<Runner*, void> _stop;
    private static bool _active;
    private static Runner* _context;

    public static void Init(delegate*<Runner*, void> start, delegate*<Runner*, void> stop, delegate*<Runner*, void> update)
    {
        _update = update;
        _start = start;
        _stop = stop;
        _thread = new Thread(RunWorker)
        {
            IsBackground = true,
            Name = "Game Loop Thread",
            Priority = ThreadPriority.Normal
        };
    }

    public static void Start(Runner* context)
    {
        _context = context;
        _active = true;
        _thread.Start();
    }

    private static void RunWorker(object _)
    {
        Logger.Trace<GameLoopThread>("Game loop thread started");
        _start(_context);
        while (_active)
        {
            _update(_context);
        }
        Logger.Trace<GameLoopThread>("Game loop thread stopped");
        _stop(_context);
    }

    public static void Stop()
    {
        _active = false;
        _thread.Join();
    }
}


public unsafe struct Runner : IApi
{
    private delegate*<delegate*<Runner*, void>, delegate*<Runner*, void>, delegate*<Runner*, void>, void> _init;
    private delegate*<Runner*, void> _start;
    private delegate*<void> _stop;

    private Scheduler* _scheduler;
    private JobApi* _jobApi;

    public static Runner Create<T>() where T : IRunner =>
        new()
        {
            _init = &T.Init,
            _start = &T.Start,
            _stop = &T.Stop,
        };


    public void Run(Scheduler* scheduler, JobApi* jobApi)
    {
        //NOTE(Jens): What should we do if it's already running?
        _scheduler = scheduler;
        _jobApi = jobApi;
        _init(&OnStart, &OnShutdown, &OnUpdate);
        _start((Runner*)Unsafe.AsPointer(ref this));
    }

    public void Stop()
    {
        _stop();
    }

    private static void OnStart(Runner* runner)
        => runner->_scheduler->Startup(*runner->_jobApi);

    private static void OnShutdown(Runner* runner)
        => runner->_scheduler->Shutdown(*runner->_jobApi);

    private static void OnUpdate(Runner* runner) 
        => runner->_scheduler->RunOnce(*runner->_jobApi);
}

public unsafe interface IRunner
{
    static abstract void Init(delegate*<Runner*, void> start, delegate*<Runner*, void> stop, delegate*<Runner*, void> update);
    static abstract void Start(Runner* context);
    static abstract void Stop();
}

public struct RunnerApi : IApi
{
    public static RunnerApi Create<T>() where T : IRunner
    {
        return new RunnerApi();
    }
}

public struct SchedulerApi : IApi, IDefault<SchedulerApi>
{
    public readonly unsafe Scheduler CreateScheduler(in TransientMemory transientMemory, in PermanentMemory permanentMemory, in SystemDescriptorCollection globalSystems, in SystemDescriptorCollection localSystems, in SchedulerConfiguration config, IApp app)
    {
        var local = localSystems.GetDescriptors();
        var global = globalSystems.GetDescriptors();
        var totalSystems = local.Length + global.Length;
        var descriptors = transientMemory.GetPointer<SystemDescriptor>((uint)totalSystems);

        // Copy the descriptors into a single array
        fixed (SystemDescriptor* pGlobal = global)
        fixed (SystemDescriptor* pLocal = local)
        {
            Unsafe.CopyBlock(descriptors, pLocal, (uint)(sizeof(SystemDescriptor) * local.Length));
            Unsafe.CopyBlock(descriptors + local.Length, pGlobal, (uint)(sizeof(SystemDescriptor) * global.Length));
        }

        var stages = SystemSchedulerFactory.Create(permanentMemory, transientMemory, new ReadOnlySpan<SystemDescriptor>(descriptors, totalSystems), app);

        return Scheduler.Create(stages, config.Get());
    }
    public static SchedulerApi Default => new();
}


public struct Scheduler
{



    private SystemExecutionStages _stages;
    private StageExecutor _preStartup;
    private StageExecutor _startup;
    private StageExecutor _preUpdate;
    private StageExecutor _update;
    private StageExecutor _postUpdate;
    private StageExecutor _shutdown;
    private StageExecutor _postShutdown;

    public void Startup(in JobApi jobApi)
    {
        _preStartup.Run(_stages.GetGraph(Stage.PreStartup), jobApi);
        _startup.Run(_stages.GetGraph(Stage.Startup), jobApi);
    }

    public void Shutdown(in JobApi jobApi)
    {
        _shutdown.Run(_stages.GetGraph(Stage.Shutdown), jobApi);
        _postShutdown.Run(_stages.GetGraph(Stage.PostShutdown), jobApi);
    }

    public void RunOnce(in JobApi jobApi)
    {
        _preUpdate.Run(_stages.GetGraph(Stage.PreUpdate), jobApi);
        _update.Run(_stages.GetGraph(Stage.Update), jobApi);
        _postUpdate.Run(_stages.GetGraph(Stage.PostUpdate), jobApi);
    }

    public static Scheduler Create(in SystemExecutionStages stages, ReadOnlySpan<StageExecutor> stageExecutors) =>
        new()
        {
            _stages = stages,
            _preStartup = stageExecutors[(int)Stage.PreStartup],
            _startup = stageExecutors[(int)Stage.Startup],
            _preUpdate = stageExecutors[(int)Stage.PreUpdate],
            _update = stageExecutors[(int)Stage.Update],
            _postUpdate = stageExecutors[(int)Stage.PostUpdate],
            _shutdown = stageExecutors[(int)Stage.Shutdown],
            _postShutdown = stageExecutors[(int)Stage.PostShutdown]
        };
}

public readonly struct SchedulerModule : IModule
{
    public static void Build(IApp app)
    {
        var config = app.GetResourceOrDefault<SchedulerConfiguration>();

        Logger.Warning<SchedulerModule>("Not using the scheduler config for anything yet");

        app.AddResource(SchedulerApi.Default)
            .AddResource(Runner.Create<GameLoopThread>());
    }
}
