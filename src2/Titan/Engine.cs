using System;
using Titan.Core.Logging;

namespace Titan
{
    public class Engine : IDisposable
    {
        private readonly Application _app;

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
            Logger.Info("Engine has been initialized.");
            Logger.Info("Initialize Application.");
            _app.OnStart();
        }

        public void Dispose()
        {
            Logger.Info("Disposing the application");
            
            Logger.Info("Disposing the engine");

            Logger.Shutdown();
        }

    }
}
