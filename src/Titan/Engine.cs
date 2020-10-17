using System;
using Titan.Core.Logging;
using Titan.Graphics.D3D11;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    internal class Engine : IEngine
    {
        private readonly IWindow _window;
        private readonly GraphicsDevice _device;
        private readonly IContainer _container;

        public Engine(EngineConfiguration configuration, IWindowFactory windowFactory, ILog log, IContainer container)
        {
            LOGGER.InitializeLogger(log);
            _window = windowFactory.Create((int) configuration.Width, (int) configuration.Height, configuration.Title);
            _device = new GraphicsDevice(_window, configuration.RefreshRate, configuration.Debug);
            
            container
                .RegisterSingleton<IGraphicsDevice>(_device)
                .RegisterSingleton(_window);


            _container = container;
        }


        public void Dispose()
        {
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
