using System;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Materials;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Resources;
using Titan.Graphics.States;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    internal class Engine : IEngine
    {
        private readonly IWindow _window;
        private readonly IGraphicsDevice _device;
        
        private readonly IContainer _container;
        private readonly IGraphicsPipeline _pipeline;
        private readonly IMemoryManager _memoryManager;

        private readonly IEventQueue _eventQueue;

        public Engine(EngineConfiguration configuration, ILog log, IContainer container)
        {
            LOGGER.InitializeLogger(log);

            container
                .RegisterSingleton(new TitanConfiguration(configuration.ResourceBasePath, configuration.RefreshRate, 0.02f, configuration.Debug))
                .RegisterSingleton(_eventQueue  = container.CreateInstance<EventQueue>())
                .RegisterSingleton(_memoryManager = CreateMemoryManager())
                .RegisterSingleton(_window = container.GetInstance<IWindowFactory>().Create((int) configuration.Width, (int) configuration.Height, configuration.Title))
                .RegisterSingleton(_device = container.CreateInstance<GraphicsDevice>())
                .RegisterSingleton(_pipeline = container.CreateInstance<GraphicsPipeline>());

            LOGGER.Debug("Initialize EventQueue with {0}", typeof(ScanningEventTypeProvider));
            _eventQueue.Initialize(new ScanningEventTypeProvider());

            LOGGER.Debug("Initialize GraphicsPipeline");
            _pipeline.Initialize("render_pipeline.json");

            _container = container;
        }

        private static unsafe IMemoryManager CreateMemoryManager()
        {
            LOGGER.Debug("Initialize memory manager");
            // TODO: not sure how to do this yet. Could have each manager "request" a memory chunk.
            return new MemoryManager(new[]
            {
                new ChunkDescriptor("VertexBuffer", (uint) sizeof(VertexBuffer), 2048),
                new ChunkDescriptor("IndexBuffer", (uint) sizeof(IndexBuffer), 2048),
                new ChunkDescriptor("ConstantBuffer", (uint) sizeof(ConstantBuffer), 100),
                new ChunkDescriptor("Materials", (uint) sizeof(Material), 256),
                new ChunkDescriptor("Shaders", (uint) IntPtr.Size, 1024),
                new ChunkDescriptor("Texture", (uint) sizeof(Graphics.Resources.Texture), 1024),
                new ChunkDescriptor("ShaderResourceView", (uint) sizeof(ShaderResourceView), 1024),
                new ChunkDescriptor("RenderTargetView", (uint) sizeof(RenderTargetView), 1024),
                new ChunkDescriptor("DepthStencilView", (uint) sizeof(DepthStencilView), 10),
                new ChunkDescriptor("DepthStencilState", (uint) sizeof(DepthStencilState), 10),
                new ChunkDescriptor("SamplerState", (uint) sizeof(SamplerState), 20),
            });
        }

        public void Dispose()
        {
            _pipeline.Dispose();
            _device.Dispose();
            _window.Dispose();

            // memory manager must be cleared last
            _memoryManager.Dispose();

            //using ComPtr<ID3D11Debug> d3dDebug = default;
            //fixed (Guid* debugGuidPtr = &D3D11Common.D3D11Debug)
            //{
            //    Common.CheckAndThrow(ptr->QueryInterface(debugGuidPtr, (void**)d3dDebug.GetAddressOf()), "QueryInterface");
            //}
            //Common.CheckAndThrow(d3dDebug.Get()->ReportLiveDeviceObjects(D3D11_RLDO_FLAGS.D3D11_RLDO_DETAIL), "ReportLiveDeviceObjects");
        }

        public void Start()
        {
            while (_window.Update())
            {
                _eventQueue.Update();

                _pipeline.Execute();
            }
        }

        public void Stop()
        {
            _window.Exit();
        }

        IWindow IEngine.Window => _window;
        IContainer IEngine.Container => _container;
    }

    
}
