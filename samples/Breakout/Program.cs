using System;
using System.Numerics;
using Breakout;
using Titan;
using Titan.Assets;
using Titan.Components;
using Titan.Core.Logging;
using Titan.ECS;
using Titan.ECS.Worlds;
using Titan.Graphics.Loaders.Models;
using Titan.Graphics.Windows;

Console.WriteLine($"Hello World!");

Engine.StartNew<BreakoutApplication>();

namespace Breakout
{
    internal class BreakoutApplication : Application
    {
        public override void OnStart(World world)
        {
            var block = world.CreateEntity();
            block.AddComponent(Transform3D.Default);
            block.AddComponent(new AssetComponent<Model>("models/block"));

            var camera = world.CreateEntity();
            camera.AddComponent(new Transform3D { Position = new Vector3(0, 10, 60), Rotation = Quaternion.Identity, Scale = Vector3.One });
            camera.AddComponent(CameraComponent.CreatePerspective(2560, 1440, 0.5f, 10000f));
        }

        public override void ConfigureWorld(WorldBuilder builder) => 
            builder
                .WithSystem(new FirstPersonCameraSystem(Window));

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
