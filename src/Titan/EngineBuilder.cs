using System;
using Titan.IOC;

namespace Titan
{
    public class EngineBuilder
    {
        private Func<string> _pathFunc = () => Environment.CurrentDirectory;
        private Func<bool> _debugFunc = () => true;
        private uint _width;
        private uint _height;
        private string _title;
        private readonly IContainer _container = Bootstrapper.CreateContainer();

        public static EngineBuilder CreateDefaultBuilder() => new();

        public EngineBuilder ConfigureResourcesBasePath(Func<string> pathFunc)
        {
            _pathFunc = pathFunc;
            return this;
        }

        public EngineBuilder ConfigureWindow(uint width, uint height, string title)
        {
            _width = width;
            _height = height;
            _title = title;
            return this;
        }

        public EngineBuilder ConfigureDebug(Func<bool> debugFunc)
        {
            _debugFunc = debugFunc;
            return this;
        }
        
        public IEngine Build()
        {
            var engineConfiguration = new EngineConfiguration
            {
                Width = _width,
                Height = _height,
                Title = _title,
                ResourceBasePath = _pathFunc(),
                RefreshRate = 144,
                Debug = _debugFunc()
            };
            return _container
                .RegisterSingleton(_container)
                .RegisterSingleton(engineConfiguration)
                .CreateInstance<Engine>();
        }
    }
}
