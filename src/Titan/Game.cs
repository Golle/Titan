using Titan.Graphics.Windows;

namespace Titan;

/// <summary>
/// Entry point for the Game
/// </summary>
public abstract class Game
{
    public virtual EngineConfiguration ConfigureEngine(EngineConfiguration config) => config;
    public virtual WindowConfiguration ConfigureWindow(WindowConfiguration config) => config;
}



public record EngineConfiguration
{
    
}
