using System;
using System.IO;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;

namespace Titan.Core.IO
{
    public record FileSystemConfiguration(string BasePath);
    
    // TODO: add different types of file systems, right now we just use a path
    public static class FileSystem
    {
        private static string BasePath;
        private static bool _initialized;

        public static void Init(FileSystemConfiguration config)
        {
            if (_initialized)
            {
                throw new InvalidOperationException($"{nameof(FileSystem)} has already been initialized.");
            }
            Logger.Trace($"Init the file system with base path: {config.BasePath}", typeof(FileSystem));

            BasePath = Path.GetFullPath(config.BasePath);

            _initialized = true;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFullPath(string identifier) => Path.GetFullPath(Path.Combine(BasePath, identifier));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exist(string identifier) => File.Exists(GetFullPath(identifier));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FileStream OpenRead(string identifer) => File.OpenRead(GetFullPath(identifer));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> ReadAllBytes(string identifier) => File.ReadAllBytes(GetFullPath(identifier));

        public static void Terminate()
        {
            Logger.Trace($"Terminate the {nameof(FileSystem)}");


            _initialized = false;
        }

    }
}
