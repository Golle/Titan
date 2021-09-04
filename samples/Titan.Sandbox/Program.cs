using System;
using System.Numerics;
using Titan;
using Titan.Assets;
using Titan.Components;
using Titan.Core.Logging;
using Titan.ECS;
using Titan.ECS.Worlds;
using Titan.Graphics;
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

            var container = new UIContainer
            {
                Offset = new Vector2(100, 100),
                Size = (1200, 600),
                ZIndex = 0,
                Pivot = new(0, 0)
            };


            // Uncomment this to render the text-align/valign boxes
            //ShowTextAlignBoxes();
            //ShowAchorPoints();
            ShowButtonIds();


            void ShowTextAlignBoxes()
            {
                var textAlign = new[] { TextAlign.Left, TextAlign.Center, TextAlign.Right };
                var verticalAlign = new[] { VerticalAlign.Bottom, VerticalAlign.Middle, VerticalAlign.Top };

                for (var x = 0; x < textAlign.Length; ++x)
                {
                    for (var y = 0; y < verticalAlign.Length; ++y)
                    {
                        container.Add(new UIText
                        {
                            AnchorPoint = AnchorPoint.Left,
                            Font = "fonts/seqoe_ui_light",
                            TextAlign = textAlign[x],
                            Color = Color.White,
                            VerticalAlign = verticalAlign[y],
                            VerticalOverflow = VerticalOverflow.Truncate,
                            HorizontalOverflow = HorizontalOverflow.Wrap,
                            Size = (300, 124),
                            Offset = new Vector2(200 + x * 400, 200 + y * 250),
                            //Text = "VerticalOverflow = Overflow Lorem ipsum dolor\nsit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                            Text = "Aipsum dolor sit amet Aipsum dolor sit amet Aipsum dolor sit amet Aipsum dolor sit amet Aipsum dolor sit amet Aipsum dolor sit amet Aipsum dolor sit ametgq",
                            LineHeight = 20,
                            FontSize = 23,
                            Pivot = Vector2.Zero,
                            ZIndex = 1
                        });
                    }
                }
            }

            void ShowAchorPoints()
            {
                var a = new[] { AnchorPoint.Top, AnchorPoint.Bottom, AnchorPoint.Middle };
                var b = new[] { AnchorPoint.Left, AnchorPoint.Right, AnchorPoint.Center };

                foreach (var vertical in a)
                {
                    foreach (var horizontal in b)
                    {
                        container.AddButton(new UIButton
                        {
                            Size = (38, 36),
                            ZIndex = 1,
                            Offset = Vector2.Zero,
                            AnchorPoint = vertical | horizontal,
                            Pivot = new(0.5f, 0.5f),
                            Sprite = new Sprite
                            {
                                Identifier = "atlas/redsheet",
                                Index = 1
                            }
                        });
                    }
                }
            }


            void ShowButtonIds()
            {
                ushort count = 0;
                for(var i =0 ; i < 4; ++i)
                    for(var j = 0; j < 4; ++j)
                        container.Add(new UIButton
                        {
                            Identifier = count,
                            Size = (120,50),
                            Sprite = new Sprite
                            {
                                Identifier = "atlas/redsheet",
                                Index = 25,
                                Margins = (10,10,10,10)
                            },
                            Offset = new (i*130, j *60),
                            AnchorPoint = AnchorPoint.BottomLeft,
                            Pivot = Vector2.Zero,
                            Text = new UIText
                            {
                                FontSize = 20,
                                LineHeight = 20,
                                Text = $"Button ({count++})",
                                Font = "fonts/seqoe_ui_light",
                                Color = Color.Blue,
                                TextAlign = TextAlign.Center,
                                VerticalAlign = VerticalAlign.Middle,
                                AnchorPoint = AnchorPoint.MiddleCenter,
                                VerticalOverflow = VerticalOverflow.Overflow,
                                HorizontalOverflow = HorizontalOverflow.Overflow
                            }
                        });
            }
            


            
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
                //Width = 2560,
                //Height = 1440,
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
