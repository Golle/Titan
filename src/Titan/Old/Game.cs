using System.Collections.Generic;
using Titan.ECS;
using Titan.ECS.Worlds;
using Titan.Graphics.D3D11;
using Titan.Graphics.Windows;
using Titan.Old.Pipeline;
using Titan.Physics;
using Titan.UI;

namespace Titan.Old;

public abstract class Game
{

    public abstract IEnumerable<WorldConfiguration> ConfigureWorlds();
    public virtual void OnStart(World starterWorld, UIManager uiManager){}
    public virtual void OnTerminate() { }
    public abstract void ConfigureStarterWorld(WorldBuilder builder);
    public abstract EngineConfiguration ConfigureEngine(EngineConfiguration config);
    public virtual WindowConfiguration ConfigureWindow(WindowConfiguration config) => config;
    public virtual DeviceConfiguration ConfigureDevice(DeviceConfiguration config) => config;
    public virtual CollisionMatrixConfiguration ConfigureCollisionMatrix() => null;
    public virtual RenderingPipeline ConfigureRenderingPipeline() => RenderingPipeline.Render3D;
}
