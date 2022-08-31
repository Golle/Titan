using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Modules;

//NOTE(Jens): Move this to Core
public readonly struct LoggingModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        ref readonly var config = ref builder.GetResourceOrDefault<LoggingConfiguration>();
        if (config.Enabled)
        {
            ILogger logger = config.Type switch
            {
                LoggerType.File => new FileLogger(config.FilePath),
                LoggerType.Console or _ => new ConsoleLogger(),
            };
            Logger.Start(logger);
            builder
                .AddSystemToStage<LoggerTeardown>(Stage.PostShutdown, priority: int.MaxValue)// Set Priority to Max to it's executed after all other systems have run their shutdown
                ;
        }

        return true;
    }

    private struct LoggerTeardown : IStructSystem<LoggerTeardown>
    {
        public static void Init(ref LoggerTeardown system, in SystemsInitializer init) { }
        public static void Update(ref LoggerTeardown system) => Logger.Shutdown();
        public static bool ShouldRun(in LoggerTeardown system) => true;
    }
}
