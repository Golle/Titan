namespace Titan.ECS;

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



    //WorldLoaded,

    First, // only used by events

    /// <summary>
    /// Systems that should be executed before the start of the "frame".
    /// Only use if you know what you're doing, this wont create an execution tree based on dependencies, it will run everything in parallel
    /// </summary>
    PreUpdate,

    /// <summary>
    /// Default stage for all systems.
    /// </summary>
    Update,

    /// <summary>
    /// Systems that should be executed at the end of the "frame".
    /// Only use if you know what you're doing, this wont create an execution tree based on dependencies, it will run everything in parallel
    /// </summary>
    PostUpdate,

    //Last // add if needed

    //NOTE(Jens): Startup/Shutdown should only be used for Global systems. Add WorldLoaded/WorldUnloaded for switching worlds
    //WorldUnloaded,

    /// <summary>
    /// When the game exits
    /// </summary>
    Shutdown,

    /// <summary>
    /// The last thing to shutdown (turn off logging for example)
    /// </summary>
    PostShutdown,

    /// <summary>
    /// The number of stages
    /// </summary>
    Count
}
