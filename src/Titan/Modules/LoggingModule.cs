using Titan.Core.Logging;
using Titan.ECS;
using Titan.ECS.App;
using Titan.ECS.SystemsV2;

namespace Titan.Modules;

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
