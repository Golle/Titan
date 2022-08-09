using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Modules;

//NOTE(Jens): Move this to Core
public readonly struct LoggingModule : IModule
{
    public static void Build(AppBuilder builder)
    {

        ref readonly var config = ref builder.GetResourceOrDefault<LoggingConfiguration>();
        if (config.Enabled)
        {
            Logger.Start();
            builder
                .AddSystemToStage<LoggerTeardown>(Stage.PostShutdown, priority: int.MaxValue)// Set Priority to Max to it's executed after all other systems have run their shutdown
                
                ;
        }
    }

    private struct LoggerTeardown : IStructSystem<LoggerTeardown>
    {
        public static void Init(ref LoggerTeardown system, in SystemsInitializer init) { }
        public static void Update(ref LoggerTeardown system) => Logger.Shutdown();
        public static bool ShouldRun(in LoggerTeardown system) => true;
    }
}
