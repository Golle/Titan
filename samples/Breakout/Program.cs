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
using Titan.Old;
using Titan.UI;

Console.WriteLine($"Hello World!");

Engine.Start(new BreakoutApplication());

namespace Breakout
{
    internal class BreakoutApplication : Game
    {
        public override void OnStart(World world, UIManager uiManager)
        {
            var block = world.CreateEntity();
            world.AddComponent(block, Transform3DComponent.Default);
            world.AddComponent(block, new AssetComponent<Model>("models/block"));

            var camera = world.CreateEntity();
            world.AddComponent(camera, new Transform3DComponent { Position = new Vector3(0, 10, 60), Rotation = Quaternion.Identity, Scale = Vector3.One });
            world.AddComponent(camera, CameraComponent.CreatePerspective(2560, 1440, 0.5f, 10000f));
        }

        public override void ConfigureStarterWorld(WorldBuilderOld builder) => 
            builder
                .WithSystem<FirstPersonCameraSystem>();

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
