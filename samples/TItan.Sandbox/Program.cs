using System;
using Titan;
using Titan.Core.Logging;
using Titan.Sandbox;

Console.WriteLine($"Hello World!");


using var app = Engine.StartNew<SandboxApplication>();


namespace Titan.Sandbox
{
    internal class SandboxApplication : Application
    {
        public override void OnStart()
        {
            Logger.Info("Sandbox application starting");
        }

        public override void OnShutdown()
        {
            Logger.Info("Sandbox application shutting down");
        }
    }
}
