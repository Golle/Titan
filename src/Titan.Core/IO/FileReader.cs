using System.IO;

namespace Titan.Core.IO
{
    internal class FileReader : IFileReader
    {
        private readonly FileSystem _fileSystem;
        public FileReader(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string ReadText(string identifier)
        {
            var fullPath = _fileSystem.GetFullPath(identifier);

            return File.ReadAllText(fullPath);
        }

        public string[] ReadLines(string identifier)
        {
            var fullPath = _fileSystem.GetFullPath(identifier);

            return File.ReadAllLines(fullPath);
        }
    }
}
