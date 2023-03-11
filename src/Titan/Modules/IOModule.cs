using Titan.Core;
using Titan.Core.IO;
using Titan.Core.IO.Platform;
using Titan.Core.Logging;
using Titan.Setup;

namespace Titan.Modules;

public struct IOModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        IFileSystem fileSystem = GlobalConfiguration.OperatingSystem switch
        {
            OperatingSystem.Windows => new FileSystem<Win32FileApi>(),
            OperatingSystem.Linux => new FileSystem<PosixFileSystem>(),
            _ => null
        };

        if (fileSystem == null)
        {
            Logger.Error<IOModule>($"OS {GlobalConfiguration.OperatingSystem} is not supported at the moment.");
            return false;
        }
        Logger.Trace<IOModule>($"Using {fileSystem.GetType().FormattedName()} for File handling");
        builder
            .AddManagedResource(fileSystem);
        return true;
    }

    public static bool Init(IApp app) => true;
    public static bool Shutdown(IApp app) => true;
}
