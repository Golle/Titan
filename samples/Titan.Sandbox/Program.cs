using System;
using System.Numerics;
using Titan;
using Titan.Assets.Models;
using Titan.Components;
using Titan.Core.Logging;
using Titan.ECS.Worlds;
using Titan.Graphics.D3D11;
using Titan.Graphics.Windows;
using Titan.Sandbox;

Console.WriteLine($"Hello World!");

Engine.StartNew<SandboxApplication>();

namespace Titan.Sandbox
{
    internal class SandboxApplication : Application
    {
        public override void OnStart(World world)
        {
            Logger.Info("Sandbox application starting");
            
            var r = new Random();
            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    {
                        var tree = world.CreateEntity();
                        var q = Quaternion.CreateFromAxisAngle(Vector3.UnitY, r.Next(0, 360));
                        tree.AddComponent(new Transform3D
                        {
                            Scale = Vector3.One * (r.NextSingle() * 5), 
                            Rotation = q, 
                            Position = new Vector3(i * (r.NextSingle() * 10 + 5), 0, j * (r.NextSingle() * 10 + 5))
                        });
                        tree.AddComponent(new AssetComponent<Model>("models/pillar"));
                    }
                }
            }

            var camera = world.CreateEntity();
            camera.AddComponent(new Transform3D { Position = new Vector3(0, 10, 60), Rotation = Quaternion.Identity, Scale = Vector3.One });
            camera.AddComponent(CameraComponent.CreatePerspective(2560, 1440, 0.5f, 10000f));
        }

        public override void OnTerminate()
        {
            Logger.Info("Sandbox application shutting down");
        }

        public override void ConfigureWorld(WorldBuilder builder) =>
            builder
                .WithSystem(new FirstPersonCameraSystem(Window));


        public override EngineConfiguration ConfigureEngine(EngineConfiguration config) =>
            config with
            {
                AssetsPath = "assets"
            };

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
                Debug = true,
                RefreshRate = 144,
                Vsync = true
            };
    }
}