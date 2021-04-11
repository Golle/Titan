using System.IO;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Serialization;

namespace Titan.Assets.Database
{

    public record AssetDescriptor(string Name, string File, string Type, string[] Dependencies, bool Preload, bool Static);
    
    public record AssetManifest(AssetDescriptor[] Assets)
    {
        public static AssetManifest CreateFromFile(string file)
        {
            Logger.Trace<AssetManifest>($"Loading assets manifest from {file}");
            var buffer = File.ReadAllBytes(FileSystem.GetFullPath(file));
            var manifest = Json.Deserialize<AssetManifest>(buffer);
            Logger.Trace<AssetManifest>($"Manifest contains {manifest.Assets.Length} assets");
            return manifest;
        }
    }
}
