using System.IO;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Serialization;

namespace Titan.Assets.Database
{

    public record AssetDescriptor(string Name, string[] Files, string Type, string[] Dependencies, bool Preload, bool Static);
    
    public record AssetManifestDescriptor(AssetDescriptor[] Assets)
    {
        public static AssetManifestDescriptor CreateFromFile(string file)
        {
            Logger.Trace<AssetManifestDescriptor>($"Loading assets manifest from {file}");
            var buffer = File.ReadAllBytes(FileSystem.GetFullPath(file));
            var manifest = Json.Deserialize<AssetManifestDescriptor>(buffer);
            Logger.Trace<AssetManifestDescriptor>($"Manifest contains {manifest.Assets.Length} assets");
            return manifest;
        }
    }
    
}
