using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.SystemsV2;

namespace Titan.Modules;
public record struct LoggingStarted;
public record struct LoggingStopped;

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

            app.AddDisposable(new DisposableAction(Logger.Shutdown));
        }
    }
}
