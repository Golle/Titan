using Titan.ECS;
using Titan.ECS.Worlds;
using Titan.Graphics.D3D11;
using Titan.Graphics.Windows;

namespace Titan
{
    public abstract class Application
    {
        public virtual void OnStart(World world){}
        public virtual void OnTerminate() { }
        public abstract void ConfigureWorld(WorldBuilder builder);
        public abstract EngineConfiguration ConfigureEngine(EngineConfiguration config);
        public virtual WindowConfiguration ConfigureWindow(WindowConfiguration config) => config;
        public virtual DeviceConfiguration ConfigureDevice(DeviceConfiguration config) => config;

        public GameWindow Window { get; internal set; }
    }
}
