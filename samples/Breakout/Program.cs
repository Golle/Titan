using System;
using Breakout;
using Titan;
using Titan.Core.Logging;

Console.WriteLine($"Hello World!");

Engine.StartNew<SandboxApplication>();

namespace Breakout
{
    internal class SandboxApplication : Application
    {
        public override void OnStart()
        {
            Logger.Info("Sandbox application starting");
        }

        public override void ConfigureSystems(SystemsCollection collection)
        {
            collection.Add(new FirstPersonCameraSystem(Window));
        }

        public override void OnTerminate()
        {
            Logger.Info("Sandbox application shutting down");
        }
    }
}
