using System;
using Breakout;
using Titan;
using Titan.Core.Logging;
using Titan.Graphics.Windows;

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

        public override EngineConfiguration ConfigureEngine(EngineConfiguration config) =>
            config with
            {
                AssetsPath = "assets"
            };

        public override void OnTerminate()
        {
            Logger.Info("Sandbox application shutting down");
        }

        public override WindowConfiguration ConfigureWindow(WindowConfiguration config) =>
            config with
            {
                Width = 1920,
                Height = 1080,
                Title = "Breakout v0.1",
                Windowed = true
            };
    }
}
