using System.IO;

namespace Titan.Core
{
    public record TitanConfiguration(string ResourceBasePath)
    {
        public string GetPath(string filename) => Path.Combine(ResourceBasePath, filename);
    }
}
