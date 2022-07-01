using Titan;
using Titan.Components;
using Titan.Core.Modules;
using Titan.Graphics;
using Titan.Graphics.Modules;


using var app = App
    .Create()
    .AddComponent<Transform3DComponent>(100)

    .AddResource(new WindowDescriptor { Height = 600, Width = 800, Resizable = true, Title = "Sandbox" })
    .AddModule<CoreModule>()
    .AddModule<WindowModule>()
    .AddModule<RenderModule>()
    .Run()
    ;



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
