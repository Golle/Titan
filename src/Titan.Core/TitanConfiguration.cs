using System.IO;

namespace Titan.Core
{
    public record TitanConfiguration(string ResourceBasePath, uint RefreshRate, bool Debug)
    {
        public string GetPath(string filename) => Path.Combine(ResourceBasePath, filename);
    }
}
