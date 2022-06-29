using System;
using Titan.Core.Events;
using Titan.Core.Logging;

namespace Titan.Core.Modules;
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


public class DisposableAction : IDisposable
{
    private readonly Action _action;
    public DisposableAction(Action action) => _action = action;
    public void Dispose() => _action?.Invoke();
}
