using Titan.Core;

namespace Titan.Modules;

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
