using Titan.Core;
using Titan.Core.App;
using Titan.Core.Logging;
using Titan.ECS.SystemsV2;

namespace Titan.ECS.AnotherTry;

public struct LoggingConfiguration : IDefault<LoggingConfiguration>
{
    // NOTE(Jens): Add logger settings. Log level, file/console etc.
    public bool Enabled;
    public static LoggingConfiguration Default =>
        new()
        {
            Enabled = true
        };
}

//NOTE(Jens): Move this to Core
public readonly struct LoggingModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        
        ref readonly var config = ref builder.GetResourceOrDefault<LoggingConfiguration>();
        if (config.Enabled)
        {
            Logger.Start();
            builder
                .AddSystemToStage<LoggerTeardown>(Stage.PostShutdown);
        }
    }

    private struct LoggerTeardown : IStructSystem<LoggerTeardown>
    {
        public static void Init(ref LoggerTeardown system, in SystemsInitializer init) { }
        public static void Update(ref LoggerTeardown system) => Logger.Shutdown();
        public static bool ShouldRun(in LoggerTeardown system) => true;
    }
}
