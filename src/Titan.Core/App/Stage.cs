namespace Titan.Core.App;

public enum Stage
{
    /// <summary>
    /// Suitable for Thread Pool and logging setups 
    /// </summary>
    PreStartup,

    /// <summary>
    /// Other startup things like file system, asset manager etc.
    /// </summary>
    Startup,

    /// <summary>
    /// Default stage for all systems
    /// </summary>
    Update,

    /// <summary>
    /// When the game exits
    /// </summary>
    Shutdown,

    /// <summary>
    /// The last thing to shutdown (turn off logging for example)
    /// </summary>
    PostShutdown
}
