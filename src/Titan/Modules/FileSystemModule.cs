using Titan.Core.IO.NewFileSystem;
using Titan.Core.Logging;
using Titan.ECS.App;

namespace Titan.Modules;

public struct FileSystemModule : IModule
{
    public static void Build(AppBuilder builder)
    {
        Logger.Trace<FileSystemModule>($"Creating a {nameof(ManagedFileSystemApi)}");
        var api = FileSystemApi.Create<ManagedFileSystemApi>();

        builder.AddResource(api);

    }
}
