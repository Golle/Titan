using System;
using Titan;
using Titan.Core.Logging;
using TItan.Sandbox;

Console.WriteLine($"Hello World!");

Engine.StartNew<SandboxApplication>();

namespace TItan.Sandbox
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
