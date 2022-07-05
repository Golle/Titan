using Titan.ECS;
using Titan.Graphics.Windows;

namespace Titan;

/// <summary>
/// Entry point for the Game
/// </summary>
public abstract class Game
{
    public virtual EngineConfiguration ConfigureEngine(EngineConfiguration config) => config;
    public virtual WindowConfiguration ConfigureWindow(WindowConfiguration config) => config;
    public virtual SystemsConfiguration ConfigureSystems(SystemsBuilder builder) => builder.Build();

    public abstract IEnumerable<WorldConfiguration> ConfigureWorlds();
}


public record EngineConfiguration
{
    public string AssetsPath { get; init; }
    /// <summary>
    /// The base path pattern is only used when running the game from Visual Studio (or any other editor)
    /// </summary>
    public string BasePathSearchPattern { get; init; }
}
