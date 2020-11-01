using System;
using System.IO;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    internal class Engine : IEngine
    {
        private readonly IWindow _window;
        private readonly GraphicsDevice _device;
        
        private readonly IContainer _container;
        private readonly IGraphicsPipeline _pipeline;

        public Engine(EngineConfiguration configuration, IWindowFactory windowFactory, IEventQueue eventQueue, ILog log, IContainer container)
        {
            LOGGER.InitializeLogger(log);
            LOGGER.Debug("Initialize EventQueue with {0}", typeof(ScanningEventTypeProvider));
            eventQueue.Initialize(new ScanningEventTypeProvider());

            _window = windowFactory.Create((int) configuration.Width, (int) configuration.Height, configuration.Title);
            _device = new GraphicsDevice(_window, configuration.RefreshRate, configuration.Debug);
            
            container
                .RegisterSingleton<IGraphicsDevice>(_device)
                .RegisterSingleton(_window)
                .RegisterSingleton(new TitanConfiguration(configuration.ResourceBasePath));
            
            _pipeline = container.GetInstance<IGraphicsPipeline>();
            LOGGER.Debug("Initialize GraphicsPipeline");
            _pipeline.Initialize("render_pipeline.json");


            _container = container;
        }

        public void Dispose()
        {
            _pipeline.Dispose();
            _device.Dispose();
            _window.Dispose();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        IGraphicsDevice IEngine.Device => _device;
        IWindow IEngine.Window => _window;

        IContainer IEngine.Container => _container;
    }

    
}
