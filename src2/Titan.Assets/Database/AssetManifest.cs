using System.IO;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Serialization;

namespace Titan.Assets.Database
{
    public record AssetDescriptor(string Name, string File, string Type, string[] Dependencies, bool Preload, bool Static);
    
    public record AssetManifest(AssetDescriptor[] Descriptors)
    {
        public static AssetManifest CreateFromFile(string file)
        {
            Logger.Trace<AssetManifest>($"Loading assets manifest from {file}");
            var buffer = File.ReadAllBytes(FileSystem.GetFullPath(file));
            var descriptors = Json.Deserialize<AssetDescriptor[]>(buffer);
            Logger.Trace<AssetManifest>($"Manifest contains {descriptors.Length} assets");
            return new AssetManifest(descriptors);
        }
    }
}
