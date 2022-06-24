using System.Collections.Generic;
using Titan;
using Titan.Components;
using Titan.ECS;
using Titan.ECS.TheNew;
using Titan.Graphics.Windows;
using Titan.Systems;

Engine.Start(new SandboxGame());

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
