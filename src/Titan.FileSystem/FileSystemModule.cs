using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.FileSystem.Platform;

namespace Titan.FileSystem;

public struct FileSystemModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        bool win32 = true;

        if (win32)
        {
            Logger.Trace<FileSystemModule>($"Creating a {nameof(Win32FileSystemApi)}");
            var api = FileSystemApi.Create<Win32FileSystemApi>();

            builder.AddResource(api);
        }
        else
        {
            Logger.Trace<FileSystemModule>($"Creating a {nameof(ManagedFileSystemApi)}");
            var api = FileSystemApi.Create<ManagedFileSystemApi>();

            builder.AddResource(api);
        }
        return true;
    }
}
