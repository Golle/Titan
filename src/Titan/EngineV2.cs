using System;
using System.Linq;
using System.Threading;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Core.Services;
using Titan.Core.Threading;
using Titan.ECS.Systems.Resources;
using Titan.ECS.TheNew;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Loaders;
using Titan.Graphics.Windows;
using Titan.Pipeline;

namespace Titan;

public class EngineV2
{
    private readonly Game _app;
    private WindowV2 _window;

    public static void Start(Game game)
    {
        try
        {
            new EngineV2(game)
                .Start();
        }
        catch
        {
            // ignored
        }
        finally
        {
            Logger.Shutdown();
        }

    }
    private EngineV2(Game app)
    {
        _app = app;
    }

    private void Start()
    {
        static void Info(string message) => Logger.Info<EngineV2>(message);
        static void Trace(string message) => Logger.Trace<EngineV2>(message);

        Logger.Start();

        Info("Starting the engine!");


        var engineConfig = _app.ConfigureEngine(new EngineConfiguration { MaxEvents = 10_0000 }); // NOTE(Jens): Events should have different channels
        if (string.IsNullOrWhiteSpace(engineConfig.AssetsPath))
        {
            Logger.Error<Engine>($"{nameof(EngineConfiguration.AssetsPath)} is not set. Must be a valid relative path.");
            return;
        }
        if (engineConfig.MaxEvents == 0)
        {
            Logger.Error<Engine>($"{nameof(EngineConfiguration.MaxEvents)} is set to 0. Must be a valid positive number.");
            return;
        }

        // Base systems
        {
            Trace($"Init {nameof(EventManager)}");
            EventManager.Init(new EventManagerConfiguration(engineConfig.MaxEvents));
            Trace($"Init {nameof(FileSystem)}");
            FileSystem.Init(new FileSystemConfiguration(engineConfig.AssetsPath, engineConfig.BasePathSearchPattern));
            Trace($"Init {nameof(WorkerPool)}");
            WorkerPool.Init(new WorkerPoolConfiguration(100, (uint)((Environment.ProcessorCount / 2) - 1)));
            Trace($"Init {nameof(IOWorkerPool)}");
            IOWorkerPool.Init(2, 100);
        }

        // Setup D3D11 and the Window
        {
            // NOTE(Jens): replace old Window/Graphics implementations with the new one and support multithreaded stuff
            Trace($"Configure the {nameof(Window)}");
            var windowConfig = _app.ConfigureWindow(new WindowConfiguration(_app.GetType().Name, 800, 600, true));
            
            Trace($"Creating the {nameof(WindowV2)}");
            _window = WindowV2.Create(windowConfig);
            if (_window == null)
            {
                Logger.Error("Failed to create the window.", typeof(Engine));
                return;
            }

            Trace($"Showing the {nameof(Window)}");
            _window.Show();

            Trace($"Configure {nameof(GraphicsDevice)}");
            var deviceConfig = _app.ConfigureDevice(new DeviceConfiguration(windowConfig.Width, windowConfig.Height, 144, windowConfig.Windowed, true, true, true));
            Trace($"Init {typeof(GraphicsDevice).FullName}");
            GraphicsDevice.Init(deviceConfig, _window.WindowHandle);
        }

        // Some old implemeentation of resources
        {
            Trace($"Init {nameof(Resources)}");
            Resources.Init();
        }
        Info("Engine has been initialized.");

        try
        {
            Run();
        }
        catch (Exception e)
        {
            Logger.Error("Exception was thrown at startup.");
            Logger.Error(e.Message);
            Logger.Error(e.StackTrace);
        }
        finally
        {
            Shutdown();
        }
    }

    private void Run()
    {
        using var services = new ServiceCollection()
                .Register<ISharedResources>(new SharedResourceManager(10 * 1024 * 1024))

            ;

        // Register all base systems
        var baseSystems = new[]
        {
            typeof(TestSystem1),
            typeof(TestSystem2),
            typeof(TestSystem3),
            typeof(TestSystem4),
        };

        var worlds = _app.ConfigureWorlds()
            .ToDictionary(w => w.Name);

        var starterWorld = worlds.First().Value;

        var systems = starterWorld
            .Systems
            .Select(s => s.Type)
            .Concat(baseSystems)
            .Select(Activator.CreateInstance)
            .Cast<BaseSystem>()
            .ToArray();

        var runner = new WorldRunner();
        runner.Start(systems);
        Logger.Info("Start windows poll on main thred");
        while (_window.Update())
        {

        }

        Logger.Info("Window returned false, exiting.");
        runner.Stop();
    }

    private void Shutdown()
    {
        Logger.Info<Engine>("Disposing the application");
        _app.OnTerminate();

        Logger.Info<Engine>("Disposing the engine");

        Logger.Trace<Engine>($"Terminate {nameof(WorkerPool)}");
        WorkerPool.Terminate();

        Logger.Trace<Engine>($"Terminate {nameof(IOWorkerPool)}");
        IOWorkerPool.Terminate();

        Logger.Trace<Engine>($"Terminate {nameof(GraphicsDevice)}");
        GraphicsDevice.Terminate();

        Logger.Trace<Engine>($"Terminate {nameof(Resources)}");
        Resources.Terminate();

        Logger.Trace<Engine>($"Close/Dispose {nameof(Window)}");
        _window.Dispose();

        Logger.Trace<Engine>($"Terminate {nameof(FileSystem)}");
        FileSystem.Terminate();

        Logger.Trace<Engine>($"Terminate {nameof(EventManager)}");
        EventManager.Terminate();
    }
}


// NOTE(Jens): this is a bad name, rename it when we know what it does
internal class WorldRunner
{
    private Thread _worldThread;
    private volatile bool _active;

    public void Start(BaseSystem[] systems)
    {
        _worldThread = new Thread(Run);
        _worldThread.Start(systems);
    }

    public void Stop()
    {
        _active = false;
        _worldThread.Join();
        // Add stop function
    }

    public void Run(object obj)
    {
        _active = true;
        Node[] nodes;
        {
            nodes = new DispatchTreeFactory()
                .Construct((BaseSystem[])obj);
        }


        var dispatcher = new SystemsDispatcher_(nodes);

        while (_active)
        {
            dispatcher.Execute();
        }
        Logger.Info("Exiting the World Thread");
    }
}
