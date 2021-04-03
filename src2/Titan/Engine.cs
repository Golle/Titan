using System;
using Titan.Core.Logging;
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
            Logger.Start();

            _window = Window.Create(new WindowConfiguration("Titan is a moon ?!", 1920, 1080));
            _window.Show();
            Logger.Info("Engine has been initialized.");
            Logger.Info("Initialize Application.");
            _app.OnStart();

            while (_window.Update())
            {
                // Do stuff with the engine
            }
        }

        public void Dispose()
        {
            Logger.Info("Disposing the application");
            
            Logger.Info("Disposing the engine");

            _window.Dispose();
            Logger.Shutdown();
        }

    }
}
