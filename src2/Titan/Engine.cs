using System;
using Titan.Core.Logging;
using Titan.Graphics.D3D11;
using Titan.Graphics.Windows;

namespace Titan
{
    public class Engine : IDisposable
    {
        private readonly Application _app;

        private Window _window;
        public static Engine StartNew<T>() where T : Application
        {
            var engine = new Engine(Activator.CreateInstance<T>());
            engine.Start();
            return engine;
        }
        
        public Engine(Application app)
        {
            _app = app;
        }

        public void Start()
        {
            static void Info(string message) => Logger.Info<Engine>(message);
            static void Trace(string message) => Logger.Trace<Engine>(message);

            Logger.Start();

            Trace("Creating the Window");
            _window = Window.Create(new WindowConfiguration("Titan is a moon ?!", 1920, 1080));
            Trace("Showing the Window");
            _window.Show();

            Trace($"Init the GraphicsDevice: {typeof(GraphicsDevice).FullName}");
            GraphicsDevice.Init(_window, new DeviceConfiguration(144, true, true));

            Info("Engine has been initialized.");
            Info("Initialize Application.");
            _app.OnStart();

            while (_window.Update())
            {
                // Do stuff with the engine
            }
        }

        public void Dispose()
        {
            Logger.Info<Engine>("Disposing the application");
            
            Logger.Info<Engine>("Disposing the engine");

            GraphicsDevice.Shutdown();
            _window.Dispose();
            Logger.Shutdown();
        }
    }
}
