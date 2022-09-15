using Titan.Core.Logging;
using Titan.ECS.App;

namespace Titan.Modules;

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
        }
        return true;
    }
}
