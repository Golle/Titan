using System.Threading;
using Titan.Components;
using Titan.Core.Logging;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;
using Titan.Graphics;
using Titan.Graphics.Modules;
using Titan.Modules;
using Titan.NewStuff;

using var app = App
    .Create(AppCreationArgs.Default)

    .AddResource(new WindowDescriptor { Height = 600, Width = 800, Resizable = true, Title = "Sandbox" })
    .AddModule<CoreModule>()
    .AddModule<WindowModule>()
    .AddModule<RenderModule>()
    .AddResource(new GlobalFrameCounter())
    .AddWorld<StartupWorld>()
    .AddWorld(config => config
        .AddComponent<Transform3DComponent>()
        .AddStartupSystem<FrameCounter>()
        .AddSystem<FrameCounter>()
        .AddSystem<PrintFrameCounter>())
    .Run()
    ;

public struct TheContext
{
    public int B;
}

public static class A
{
    public static void B(ref TheContext context)
    {
        var newValue = Interlocked.Increment(ref context.B);
        Thread.Sleep(10);
        //Logger.Error($"New value: {newValue}");
    }
}

internal struct FrameCounter : IStructSystem<FrameCounter>
{
    private MutableResource<GlobalFrameCounter> _global;

    public static void Init(ref FrameCounter system, in SystemsInitializer init)
    {
        system._global = init.GetMutableResource<GlobalFrameCounter>();
    }

    public static void Update(ref FrameCounter system)
    {
        system._global.Get().FrameCounter++;
    }
}

internal struct PrintFrameCounter : IStructSystem<PrintFrameCounter>
{
    private ReadOnlyResource<GlobalFrameCounter> _global;
    public static void Init(ref PrintFrameCounter system, in SystemsInitializer init)
    {
        system._global = init.GetReadOnlyResource<GlobalFrameCounter>();
    }

    public static void Update(ref PrintFrameCounter system)
    {
        var count = system._global.Get().FrameCounter;

        Logger.Trace<PrintFrameCounter>($"Current frame count: {count}");
    }
}


public readonly struct StartupWorld : IWorldModule
{
    public static void Build(WorldConfig config)
    {
        config
            .AddSystem<PrintFrameCounter>()
            .AddSystem<FrameCounter>();
    }
}

struct GlobalFrameCounter
{
    public long FrameCounter;
}


//internal class SandboxGame : Game
//{
//    public override EngineConfiguration ConfigureEngine(EngineConfiguration config) => config with
//    {
//        AssetsPath = "assets",
//        BasePathSearchPattern = "Titan.Sandbox.csproj"
//    };

//    public override WindowConfiguration ConfigureWindow(WindowConfiguration config) =>
//        config with
//        {
//            Height = 300,
//            Width = 400,
//            RawInput = false,
//            Resizable = false,
//            Windowed = true
//        };

//    public override SystemsConfiguration ConfigureSystems(SystemsBuilder builder) =>
//        builder
//            .Build();

//    public override IEnumerable<WorldConfiguration> ConfigureWorlds()
//    {
//        yield return new WorldConfigurationBuilder(10_000)
//            .WithComponent<Transform3DComponent>()
//            .WithSystem<Transform3DSystem>()
//            .Build("Game");

//    }
//}
