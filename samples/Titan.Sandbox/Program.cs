using System;
using Titan;
using Titan.Core.Logging;
using Titan.Graphics.D3D11;
using Titan.Graphics.Windows;
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

        public override void ConfigureSystems(SystemsCollection collection)
        {
            collection.Add(new FirstPersonCameraSystem(Window));
        }

        public override WindowConfiguration ConfigureWindow(WindowConfiguration config) =>
            config with
            {
                Height = 1080,
                Width = 1920,
                Title = "Sandbox",
                Windowed = true
            };

        public override DeviceConfiguration ConfigureDevice(DeviceConfiguration config) =>
            config with
            {
                Debug = false,
                RefreshRate = 144,
                Vsync = false
            };
    }
}
