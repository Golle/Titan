using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.AnotherTry;
using Titan.ECS.Modules;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;
using Titan.EventNewer;
using Titan.Graphics.Modules;
using Titan.Input;
using Titan.Input.Modules;


AppBuilder
    .Create()
    .AddResource(SchedulerConfiguration.Default)
    .AddModule<CoreModule>()
    .AddModule<WindowModule>()
    .AddModule<InputModule>()
    .UseRunner<WindowRunner>()

    .AddSystem<FrameCounter>()
    .AddSystem<PrintFrameCounter>()
    .Build()
    .Run();


//using var app = App
//    .Create(AppCreationArgs.Default)
//    .AddModule<CoreModule>()
//    .AddResource(new WindowDescriptor { Height = 600, Width = 800, Resizable = true, Title = "Sandbox" })
//    .AddModule<WindowModule>()
//    .AddModule<InputModule>()
//    .AddModule<RenderModule>()
//    .AddSystemToStage<FrameCounter>(Stage.PreUpdate)
//    .AddSystem<PrintFrameCounter>()
//    .AddSystemToStage<FrameCounter>(Stage.PostUpdate)
//    .AddResource(new GlobalFrameCounter())
//    .Run()
//    ;

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

    public static bool ShouldRun(in FrameCounter system) => true;
}

internal struct PrintFrameCounter : IStructSystem<PrintFrameCounter>
{
    private ReadOnlyResource<GlobalFrameCounter> _global;
    private ReadOnlyResource<KeyboardState> _keyState;
    private int _counter;

    public static void Init(ref PrintFrameCounter system, in SystemsInitializer init)
    {
        system._global = init.GetReadOnlyResource<GlobalFrameCounter>();
        system._keyState = init.GetReadOnlyResource<KeyboardState>();
        system._counter = 0;
    }

    public static void Update(ref PrintFrameCounter system)
    {
        ref readonly var keyboardState = ref system._keyState.Get();
        if (keyboardState.IsKeyReleased(KeyCode.A))
        {
            Logger.Warning<PrintFrameCounter>("Key released");
        }
        if (keyboardState.IsKeyPressed(KeyCode.A))
        {
            Logger.Warning<PrintFrameCounter>("Key pressed");
        }

        if (keyboardState.IsKeyDown(KeyCode.S))
        {
            system._counter++;
        }
        else if (keyboardState.IsKeyReleased(KeyCode.S))
        {
            Logger.Trace<PrintFrameCounter>($"Keycount: {system._counter}");
        }
    }

    public static bool ShouldRun(in PrintFrameCounter system) => true;
}


struct GlobalFrameCounter : IResource
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
