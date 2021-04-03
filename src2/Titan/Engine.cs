using System;
using Titan.Core.Logging;

namespace Titan
{
    public class Engine : IDisposable
    {
        public Engine()
        {
            Logger.Start();

            Logger.Info("Engine has been initialized.");
        }

        public void Dispose()
        {
            Logger.Info("Disposing the engine");
            Logger.Shutdown();
        }

    }
}
