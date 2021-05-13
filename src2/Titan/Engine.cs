using System;
using System.Diagnostics;
using System.Numerics;
using Titan.Assets;
using Titan.Assets.Materials;
using Titan.Assets.Models;
using Titan.Assets.Shaders;
using Titan.Components;
using Titan.Core;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Core.Threading;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Images;
using Titan.Graphics.Windows;
using Titan.Input;
using Titan.Rendering;
using Titan.Systems;

namespace Titan
{
    public class Engine
    {
        private readonly Application _app;

        private Window _window;
        public static void StartNew<T>() where T : Application
        {
            try
            {
                new Engine(Activator.CreateInstance<T>())
                    .Start();
            }
            catch
            {
                // ignored
            }
        }

        private Engine(Application app)
        {
            _app = app;
        }

        public void Start()
        {
            static void Info(string message) => Logger.Info<Engine>(message);
            static void Trace(string message) => Logger.Trace<Engine>(message);

            Logger.Start();

            Trace($"Init {nameof(EventManager)}");
            EventManager.Init(new EventManagerConfiguration(10_000));

            Trace($"Init {nameof(FileSystem)}");
            FileSystem.Init(new FileSystemConfiguration(@"f:\git\titan\assetsv2"));

            Trace($"Init {nameof(WorkerPool)}");
            WorkerPool.Init(new WorkerPoolConfiguration(100, (uint) ((Environment.ProcessorCount/2) - 1)));
            
            Trace($"Init {nameof(IOWorkerPool)}");
            IOWorkerPool.Init(2, 100);
            

            Trace($"Creating the {nameof(Window)}");
            _window = Window.Create(new WindowConfiguration("Titan is a moon ?!", 1920, 1080));
            Trace($"Showing the {nameof(Window)}");
            _window.Show();

            Trace($"Init {typeof(GraphicsDevice).FullName}");
            GraphicsDevice.Init(_window, new DeviceConfiguration(144, true, true));

            Info("Engine has been initialized.");
            Info("Initialize Application.");
            _app.OnStart();


            try
            {
                Run();
            }
            finally
            {
                Shutdown();
            }
        }

        private unsafe void Run()
        {
            var assetsManager = new AssetsManager()
                .Register(AssetTypes.Texture, new TextureLoader(new WICImageLoader()))
                .Register(AssetTypes.Model, new ModelLoader())
                .Register(AssetTypes.Shader, new ShaderLoader())
                .Register(AssetTypes.VertexShader, new VertexShaderLoader())
                .Register(AssetTypes.PixelShader, new PixelShaderLoader())
                .Register(AssetTypes.Material, new MaterialsLoader())
                .Init(new AssetManagerConfiguration("manifest.json", 2));


            var renderQueue = new SimpleRenderQueue(1000);

            
            
            Handle<Asset> asset = 0;
            var color = stackalloc float[4];
            color[0] = 1f;
            color[1] = 0.4f;
            color[2] = 0f;
            color[3] = 1f;

            var timer = Stopwatch.StartNew();
            var frameCount = 0;
            
            var pipelineBuilder = new PipelineBuilder(assetsManager, renderQueue);
            pipelineBuilder.LoadResources();
            // Preload assets for rendering pipeline
            while (_window.Update() && !pipelineBuilder.IsReady())
            {
                assetsManager.Update();
            }
            
            var pipeline = pipelineBuilder.Create();
            using var graphicsSystem = new GraphicsSystem(pipeline);

            using var world = new World(new WorldConfiguration(10_000, new[]
                {
                    new ComponentConfiguration(typeof(Transform3D), ComponentPoolTypes.Packed),
                    new ComponentConfiguration(typeof(CameraComponent), ComponentPoolTypes.Packed)
                },
                new EntitySystem[]
                {
                    new Transform3DSystem(),
                    new Render3DSystem(assetsManager, renderQueue),
                    new CameraSystem(graphicsSystem),
                    new FirstPersonCameraSystem()
                }
            ));



            var entity1 = world.CreateEntity();
            entity1.AddComponent(new Transform3D
            {
                Position = Vector3.One,
                Scale = Vector3.One,
                Rotation = Quaternion.Identity
            });

            var entity2 = world.CreateEntity();
            entity2.AddComponent(new Transform3D{Position = Vector3.Zero, Rotation = Quaternion.Identity, Scale = Vector3.One});
            entity2.AddComponent(CameraComponent.CreatePerspective(2560, 1440, 0.5f, 10000f));



            // star the main loop
            while (_window.Update())
            {
                renderQueue.Update();

                EventManager.Update();
                InputManager.Update();


                world.Update();

                if (timer.Elapsed.Seconds >= 1f)
                {
                    var elapsed = timer.Elapsed;
                    
                    _window.SetTitle($"FPS: {(int)(frameCount/elapsed.TotalSeconds)}");

                    timer.Restart();
                    frameCount = 0;
                }

                //if (count-- == 0)
                //{
                //    asset = assetsManager.Load("models/tree");
                //}

                //if (assetsManager.IsLoaded(asset))
                //{
                //    //Logger.Trace<Engine>("Asset is loaded");
                //    //var texture = assetsManager.GetAssetHandle<Texture>(asset);
                //    //Logger.Trace<Engine>($"Texture handle: {texture.Value}"); 
                //    assetsManager.Unload("models/tree");
                //}

                //if (InputManager.IsKeyPressed(KeyCode.S))
                //{
                //    entity1.AddComponent(new Transform3D
                //    {
                //        Position = new Vector3(2,43,5)
                //    });
                //} 
                //if (InputManager.IsKeyPressed(KeyCode.Space))
                //{
                //    Logger.Error("SPACE IS DOWN you smerk!");
                //    entity1.RemoveComponent<Transform3D>();
                //}
                
                assetsManager.Update();

                // Do stuff with the engine
                //GraphicsDevice.ImmediateContext.ClearRenderTarget(GraphicsDevice.SwapChain.Backbuffer, color);
                //GraphicsDevice.SwapChain.Present();
                graphicsSystem.Render();

                frameCount++;
            }
        }

        private void Shutdown()
        {
            Logger.Info<Engine>("Disposing the application");
            _app.OnTerminate();

            Logger.Info<Engine>("Disposing the engine");

            Logger.Trace<Engine>($"Terminate {nameof(WorkerPool)}");
            WorkerPool.Terminate();

            Logger.Trace<Engine>($"Terminate {nameof(IOWorkerPool)}");
            IOWorkerPool.Terminate();
            
            Logger.Trace<Engine>($"Terminate {nameof(GraphicsDevice)}");
            GraphicsDevice.Terminate();

            Logger.Trace<Engine>($"Close/Dispose {nameof(Window)}");
            _window.Dispose();

            Logger.Trace<Engine>($"Terminate {nameof(FileSystem)}");
            FileSystem.Terminate();

            Logger.Trace<Engine>($"Terminate {nameof(EventManager)}");
            EventManager.Terminate();


            Logger.Shutdown();
        }
    }

}
