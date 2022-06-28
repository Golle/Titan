using System.Collections.Generic;
using Titan;
using Titan.Components;
using Titan.Core.Logging;
using Titan.ECS;
using Titan.ECS.TheNew;
using Titan.Graphics.Windows;
using Titan.Modules;
using Titan.Systems;

//Engine.Start(new SandboxGame());
Logger.Start();
{
    using var app = App.Create()
        .AddResource(new WindowDescriptor { Height = 600, Width = 800, Resizable = true, Title = "Sandbox" })
        .WithModule<CoreModule>()
        .WithModule<WindowModule>();

    ref readonly var desc = ref app.GetResource<WindowDescriptor>();
}



Logger.Shutdown();

internal class SandboxGame : Game
{
    public override EngineConfiguration ConfigureEngine(EngineConfiguration config) => config with
    {
        AssetsPath = "assets",
        BasePathSearchPattern = "Titan.Sandbox.csproj"
    };

    public override WindowConfiguration ConfigureWindow(WindowConfiguration config) =>
        config with
        {
            Height = 300,
            Width = 400,
            RawInput = false,
            Resizable = false,
            Windowed = true
        };

    public override SystemsConfiguration ConfigureSystems(SystemsBuilder builder) =>
        builder
            .WithSystem<SandboxTestSystem>()
            .Build();

    public override IEnumerable<WorldConfiguration> ConfigureWorlds()
    {
        yield return new WorldConfigurationBuilder(10_000)
            .WithComponent<Transform3DComponent>()
            .WithSystem<Transform3DSystem>()
            .Build("Game");

    }
}



internal class SandboxTestSystem : BaseSystem
{
    protected override void OnUpdate()
    {
        // noop
    }
}
