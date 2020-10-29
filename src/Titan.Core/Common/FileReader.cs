using System.IO;

namespace Titan.Core.Common
{
    internal class FileReader : IFileReader
    {
        public string ReadText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
