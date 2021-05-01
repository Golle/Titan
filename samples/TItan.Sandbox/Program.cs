using System;
using Titan;
using Titan.Core.Logging;
using Titan.Sandbox;

Console.WriteLine($"Hello World!");

Engine.StartNew<SandboxApplication>();

namespace Titan.Sandbox
{
    internal class SandboxApplication : Application
    {
        public override void OnStart()
        {
            Logger.Info("Sandbox application starting");
        }

        public override void OnTerminate()
        {
            Logger.Info("Sandbox application shutting down");
        }
    }
}
