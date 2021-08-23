using System;
using System.Numerics;
using Titan;
using Titan.Assets;
using Titan.Components;
using Titan.Core.Logging;
using Titan.ECS;
using Titan.ECS.Worlds;
using Titan.Graphics.D3D11;
using Titan.Graphics.Loaders.Models;
using Titan.Graphics.Windows;
using Titan.Sandbox;
using Titan.UI;
using Titan.UI.Common;
using Titan.UI.Text;

Console.WriteLine($"Hello World!");

Engine.StartNew<SandboxApplication>();

namespace Titan.Sandbox
{
    internal class SandboxApplication : Application
    {
        public override void OnStart(World world, UIManager uiManager)
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

            var container = new UIPanel
            {
                Offset = new Vector2(100, 250),
                Size = (1200, 300),
                ZIndex = 0,
                Pivot = new(0, 0),
                Background = new Sprite
                {
                    Identifier = "atlas/redsheet",
                    Index = 25,
                    Margins = (12, 22, 12, 12)
                }
            };

            container.Add(new UIText
            {
                AnchorPoint = AnchorPoint.MiddleCenter,
                Font = "fonts/seqoe_ui_light",
                TextAlign = TextAlign.Center,
                Size = (800, 200),
                Offset = new Vector2(20, 0),
                Text = "Some awesome text! 123",
                LineHeight = 20,
                Pivot = new(0.5f,0.5f),
                ZIndex = 1
            });
            
            //var a = new[] { AnchorPoint.Top, AnchorPoint.Bottom, AnchorPoint.Middle };
            //var b = new[] { AnchorPoint.Left, AnchorPoint.Right, AnchorPoint.Center };

            //foreach (var vertical in a)
            //{
            //    foreach (var horizontal in b)
            //    {
            //        container.AddButton(new UIButton
            //        {
            //            Size = (38, 36),
            //            ZIndex = 1,
            //            Offset = Vector2.Zero,
            //            AnchorPoint = vertical | horizontal,
            //            Pivot = new(0.5f, 0.5f),
            //            Sprite = new Sprite
            //            {
            //                Identifier = "atlas/redsheet",
            //                Index = 1
            //            }
            //        });
            //    }
            //}



            //container.AddButton(new UIButton
            //{
            //    Size = (38, 36),
            //    ZIndex = 1,
            //    Offset = Vector2.Zero,
            //    AnchorPoint = AnchorPoint.Top | AnchorPoint.Center,
            //    Pivot = new (0.5f,0.5f),
            //    //Pivot = new (1,1),
            //    Sprite = new Sprite
            //    {
            //        Identifier = "atlas/redsheet", 
            //        Index = 1
            //    }
            //});

            //container.Add(new UIButton
            //{
            //    Offset = new (115, 30),
            //    Size = (190, 45),
            //    ZIndex = 1,
            //    Sprite = new Sprite
            //    {
            //        Identifier = "atlas/redsheet", 
            //        Index = 3
            //    }
            //});



            //container.AddButton(new UIButton
            //{
            //    Offset = Vector2.Zero,
            //    Size = (100, 100),
            //    ZIndex = 10,
            //    Sprite = new Sprite { Identifier = "atlas/ui_01", Index = 0 }
            //});

            //container.AddButton(new UIButton
            //{
            //    Offset = Vector2.One*200,
            //    Size = (190, 45),
            //    Sprite = new Sprite{ Identifier = "atlas/redsheet", Index = 4},
            //    ZIndex = 100
            //});

            //container.AddButton(new UIButton
            //{
            //    Offset = new Vector2(400, 300),
            //    Size = (100, 100),
            //    ZIndex = 1,
            //    Sprite = new Sprite { Identifier = "atlas/ui_01", Index = 0 },
            //    Text= new UIText
            //    {
            //        Font = "fonts/seqoe_ui_light",
            //        Text = "this is my text"
            //    }
            //});

            //container.AddButton(new UIButton
            //{
            //    Offset = new Vector2(600, 300),
            //    Size = (150, 150),
            //    ZIndex = 1,
            //    Sprite = new Sprite { Identifier = "atlas/ui_01", Index = 4, Margins = 20 }
            //});

            //container.Add(new UIText
            //{
            //    Size = new Size(30, 30),
            //    Offset = new Vector2(100, 00),
            //    Font = "fonts/seqoe_ui_light",
            //});
            //container.AddButton(new UIButton
            //{
            //    Offset = new Vector2(400, 200),
            //    Size = (100, 100),
            //    ZIndex = 1,
            //    Sprite = new Sprite { Identifier = "atlas/ui_01", Index = 4, Type = SpriteType.Slice, Margins = 40}
            //});

            //container.AddButton(new UIButton
            //{
            //    Offset = Vector2.One* 120,
            //    Size = (100, 150),
            //    ZIndex = 1,
            //    Sprite = new Sprite { Identifier = "atlas/ui_01", Index = 2 }
            //});
            //container.AddButton(new UIButton
            //{
            //    Offset = Vector2.One * 240,
            //    Size = (100, 125),
            //    ZIndex = 1,
            //    Sprite = new Sprite { Identifier = "atlas/ui_01", Index = 3 },

            //});
            uiManager.Add(container);
        }

        public override void OnTerminate()
        {
            Logger.Info("Sandbox application shutting down");
        }

        public override void ConfigureWorld(WorldBuilder builder) =>
            builder
                .WithSystem(new FirstPersonCameraSystem(Window))
            ;


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
                //Height = 768,
                //Width = 1024,
                Title = "Sandbox",
                Windowed = true
            };

        public override DeviceConfiguration ConfigureDevice(DeviceConfiguration config) =>
            config with
            {
                //Debug = false,
                Debug = true,
                RefreshRate = 144,
                //Vsync = false,
                Vsync = true
            };
    }

    
}
