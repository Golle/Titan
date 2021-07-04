using Titan.Graphics.D3D11;
using Titan.Graphics.Windows;

namespace Titan
{
    
    public abstract class Application
    {
        public virtual void OnStart() { }
        public virtual void OnTerminate() { }
        public virtual void ConfigureSystems(SystemsCollection collection) { }

        public abstract EngineConfiguration ConfigureEngine(EngineConfiguration config);
        public virtual WindowConfiguration ConfigureWindow(WindowConfiguration config) => config;
        public virtual DeviceConfiguration ConfigureDevice(DeviceConfiguration config) => config;

        public GameWindow Window { get; internal set; }
    }
}
