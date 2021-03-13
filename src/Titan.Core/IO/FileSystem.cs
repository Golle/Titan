using System;
using System.IO;

namespace Titan.Core.IO
{
    public record FileSystemConfiguration(string BasePath, bool Validate);
    public class FileSystem
    {
        private string _basePath;
        private bool _validate;

        public void Initialize(FileSystemConfiguration configuration)
        {
            _basePath = configuration.BasePath;
            _validate = configuration.Validate;
        }

        public string GetFullPath(string identifier)
        {
            if (string.IsNullOrWhiteSpace(_basePath))
            {
                throw new InvalidOperationException($"{nameof(FileSystem)} has not been initialized.");
            }
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier), "Must specify a file");
            }

            if (_validate)
            {
                ValidatePath(identifier);
            }

            return Path.Combine(_basePath, identifier);
        }

        private static void ValidatePath(string identifier)
        {
            if (identifier.Contains("../") || identifier.Contains(@"..\"))
            {
                throw new InvalidOperationException("Can't use ../ to traverse file paths.");
            }
        }
    }
}
