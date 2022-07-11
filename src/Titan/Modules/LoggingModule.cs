using Titan.Core.App;
using Titan.Core.Logging;
using Titan.ECS.SystemsV2;

namespace Titan.Modules;

public struct LoggingDescriptor : IDefault<LoggingDescriptor>
{
    public bool Enabled;
    public static LoggingDescriptor Default() =>
        new()
        {
            Enabled = true
        };
}
public struct LoggingModule : IModule
{
    public static void Build(IApp app)
    {
        if (!app.HasResource<LoggingDescriptor>())
        {
            app.AddResource(LoggingDescriptor.Default());
        }

        ref readonly var desc = ref app.GetResource<LoggingDescriptor>();
        if (desc.Enabled)
        {
            Logger.Start();

            app.AddSystemToStage<LoggerTeardown>(Stage.PostShutdown);
        }
    }

    private struct LoggerTeardown : IStructSystem<LoggerTeardown>
    {
        public static void Init(ref LoggerTeardown system, in SystemsInitializer init) { }
        public static void Update(ref LoggerTeardown system)
        {
            Logger.Trace<LoggerTeardown>("Shutting down logger");
            Logger.Shutdown();
        }

        public static bool ShouldRun(in LoggerTeardown system) => true;
    }
}
