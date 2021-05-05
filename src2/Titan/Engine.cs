using System;
using Titan.Assets;
using Titan.Assets.Materials;
using Titan.Assets.Models;
using Titan.Core;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Core.Threading;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Images;
using Titan.Graphics.Windows;

namespace Titan
{
    public class Engine
    {
        private readonly Application _app;

        private Window _window;
        public static void StartNew<T>() where T : Application
        {
            new Engine(Activator.CreateInstance<T>())
                .Start();
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

            var graphicsSystem = GraphicsSystem.Create();

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

            var count = 300;

            Handle<Asset> asset = 0;
            var color = stackalloc float[4];
            color[0] = 1f;
            color[1] = 0.4f;
            color[2] = 0f;
            color[3] = 1f;

            while (_window.Update())
            {
                EventManager.Update();

                if (count-- == 0)
                {
                    asset = assetsManager.Load("models/tree");

                    EventManager.Push(new SimpleEvent
                    {
                        Count = 100
                    });
                }

                if (assetsManager.IsLoaded(asset))
                {
                    //Logger.Trace<Engine>("Asset is loaded");
                    //var texture = assetsManager.GetAssetHandle<Texture>(asset);
                    //Logger.Trace<Engine>($"Texture handle: {texture.Value}"); 
                    assetsManager.Unload("models/tree");

                }
                foreach (ref readonly var @event in EventManager.GetEvents())
                {
                    Logger.Warning<Engine>($"Event type: {@event.Type} ({@event})");
                    if (@event.Type == SimpleEvent.Id)
                    {
                        ref readonly var simple = ref @event.As<SimpleEvent>();
                        Logger.Warning<Engine>($"Simple: {simple.Count}");
                    }
                }


                //t.Update();
                assetsManager.Update();

                // Do stuff with the engine
                GraphicsDevice.ImmediateContext.ClearRenderTarget(GraphicsDevice.SwapChain.Backbuffer, color);
                GraphicsDevice.SwapChain.Present();
            }
        }

        struct SimpleEvent
        {
            public static readonly short Id = EventId<SimpleEvent>.Value;
            public int Count;
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

            FileSystem.Terminate();

            Logger.Trace<Engine>($"Terminate {nameof(EventManager)}");
            EventManager.Terminate();


            Logger.Shutdown();
        }
    }

}
