using System.Runtime.CompilerServices;
using Titan.Core.Logging;

namespace Titan.Core.IO
{
    public record FileSystemConfiguration(string BasePath, string BasePathIdentifierPattern);
    
    // TODO: add different types of file systems, right now we just use a path
    public static class FileSystem
    {
        private static string _basePath;
        private static bool _initialized;

        public static void Init(FileSystemConfiguration config)
        {
            if (_initialized)
            {
                throw new InvalidOperationException($"{nameof(FileSystem)} has already been initialized.");
            }
            Logger.Trace($"Init the file system with base path: {config.BasePath}", typeof(FileSystem));

            var assetsPath = Path.GetFullPath(config.BasePath);

            if (!Directory.Exists(assetsPath))
            {
                Logger.Warning($"Asset path '{assetsPath}' does not exist, this could be because the project is running inside visual studio or with dotnet run. Trying to find the root folder.", typeof(FileSystem));

                var maxDepth = 5;
                var path = Directory.GetCurrentDirectory();

                var basePathIdentifier = config.BasePathIdentifierPattern ?? "*.csproj";
                do
                {
                    path = Directory.GetParent(path)?.FullName;
                } while (--maxDepth > 0 && path != null && Directory.GetFiles(path, basePathIdentifier, SearchOption.TopDirectoryOnly).Length == 0);

                if (path == null)
                {
                    Logger.Error("Could not find the root folder.", typeof(FileSystem));
                    throw new InvalidOperationException("Failed to locate the assets folder.");
                }
                
                assetsPath = Path.Combine(path, config.BasePath);
                if (!Directory.Exists(assetsPath))
                {
                    Logger.Error($"Asset path '{assetsPath}' does not exist. ", typeof(FileSystem));
                    throw new InvalidOperationException("Failed to locate the assets folder.");
                }
            }
            _basePath = assetsPath;
            _initialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFullPath(string identifier) => Path.GetFullPath(Path.Combine(_basePath, identifier));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exist(string identifier) => File.Exists(GetFullPath(identifier));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FileStream OpenRead(string identifer) => File.OpenRead(GetFullPath(identifer));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FileHandle OpenReadHandle(string identifier) => new(File.OpenHandle(GetFullPath(identifier)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> ReadAllBytes(string identifier) => File.ReadAllBytes(GetFullPath(identifier));

        public static void Terminate()
        {
            _initialized = false;
        }
    }
}
