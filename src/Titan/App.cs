using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Modules;
using Titan.Resources;
using Titan.Setup;
using Titan.Setup.Configs;

namespace Titan;

public record AppCreationArgs : IConfiguration
{
    public nuint ReserveVirtualMemory { get; init; } = DefaultVirtualMemory;
    public nuint GeneralPurposeMemory { get; init; } = DefaultGeneralPurposeMemory;
    public uint UnmanagedResourceTypes { get; init; } = DefaultUnmanagedResourceTypes;
    public uint UnmanagedResourceMemory { get; init; } = DefaultUnmanagedResourceMemory;

    public const nuint DefaultGeneralPurposeMemory = 512 * 1024 * 1024;
    public const nuint DefaultVirtualMemory = 2u * 1024 * 1024 * 1024;
    public const uint DefaultUnmanagedResourceTypes = 200;
    public const uint DefaultUnmanagedResourceMemory = 100 * 1024 * 1024;
}

public class App : IApp
{
    private readonly IConfiguration[] _configs;
    private readonly ResourceCollection _resources;
    private readonly ModuleLifetime[] _modules;
    private readonly IRunner _runner;

    internal App(IRunner runner, ModuleLifetime[] modules, IConfiguration[] configs, ResourceCollection resources)
    {
        _runner = runner;
        _modules = modules;
        _configs = configs;
        _resources = resources;
    }

    public static IAppBuilder Create(AppCreationArgs args)
        => new AppBuilder(args)
            .AddModule<CoreModule>();

    public void Run()
    {
#if DEBUG
        var times = new Dictionary<string, double>();
        var initTimer = Stopwatch.StartNew();
#endif

        foreach (ref readonly var module in _modules.AsSpan())
        {
            Logger.Trace<App>($"Init {module.Type.Name}");
            if (!module.Init(this))
            {
                Logger.Error<App>($"Failed to start the application. Module {module.Type.Name} failed to initialize.");
                // should do a quit (and maybe cleanup?)
                return;
            }
            Logger.Trace<App>($"Init {module.Type.Name} Finished");
#if DEBUG
            times.Add(module.Type.Name, initTimer.Elapsed.TotalMilliseconds);
            initTimer.Restart();
#endif
        }

#if DEBUG
        PrintTimers(times);
#endif


        if (!_runner.Init(this))
        {
            throw new Exception($"Failed to initialize the Runner of type {_runner.GetType().Name}");
        }

        var timer = Stopwatch.StartNew();
        var frames = 0;
        while (_runner.RunOnce())
        {
            // do stuff or just wait for an exit

            frames++;
            if (timer.Elapsed.TotalSeconds >= 1.0f)
            {
                timer.Restart();
                //Logger.Error($"FPS: {frames}");
                frames = 0;
            }
        }

        for (var i = _modules.Length - 1; i >= 0; i--)
        {
            ref readonly var module = ref _modules[i];
            if (!module.Shutdown(this))
            {
                Logger.Error<App>($"Failed to shutdown module {module.Type.Name}");
            }
        }

        var memorymanager = (MemoryManager)GetManagedResource<IMemoryManager>();
        _resources.Shutdown();
        memorymanager.Shutdown();

        GCHandleCounter.Assert();
    }

    public IEnumerable<T> GetConfigs<T>() where T : IConfiguration =>
        _configs
            .Where(c => c.GetType() == typeof(T))
            .Cast<T>();

    public unsafe ref T GetResource<T>() where T : unmanaged => ref *GetResourcePointer<T>();

    public unsafe T* GetResourcePointer<T>() where T : unmanaged => _resources.GetPointer<T>();

    public ObjectHandle<T> GetManagedResourceHandle<T>() where T : class
        => _resources.GetManaged<T>();
    public T GetManagedResource<T>() where T : class
    {
        var objectHandle = GetManagedResourceHandle<T>();
        Debug.Assert(objectHandle.Value != null);
        return objectHandle.Value!;
    }

    public T GetConfigOrDefault<T>() where T : IConfiguration, IDefault<T> => GetConfig<T>() ?? T.Default;
    public T GetConfig<T>() where T : IConfiguration => (T)_configs.FirstOrDefault(c => c.GetType() == typeof(T));


    [Conditional("DEBUG")]
    public static void PrintTimers(IDictionary<string, double> timers)
    {
        //NOTE(Jens): If we can do this without GC allocated memory it would be nice, then we can enable it for non debug builds.
        Logger.Trace<App>("System Init time");
        foreach (var timer in timers)
        {
            Logger.Trace<App>($"{timer.Key}: {timer.Value} ms");
        }
    }
}
