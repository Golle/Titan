using System.IO;
using System.Text;

namespace Titan.Core.IO
{
    internal class FileReader : IFileReader
    {
        private readonly FileSystem _fileSystem;
        public FileReader(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string ReadText(string identifier) => ReadText(identifier, Encoding.UTF8);
        public string ReadText(string identifier, Encoding encoding)
        {
            var fullPath = _fileSystem.GetFullPath(identifier);
            return File.ReadAllText(fullPath, encoding);
        }

        public string[] ReadLines(string identifier)
        {
            var fullPath = _fileSystem.GetFullPath(identifier);
            return File.ReadAllLines(fullPath);
        }
    }
}
