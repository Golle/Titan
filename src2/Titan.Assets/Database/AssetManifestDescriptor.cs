using System.Diagnostics;
using System.IO;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Serialization;

namespace Titan.Assets.Database
{
    public record AssetDescriptor(string Id, string[] Files, string Type, AssetDependencyDescriptor[] Dependencies, bool Preload, bool Static);
    public record AssetDependencyDescriptor(string Id, string Name);

    public record AssetManifestDescriptor(AssetDescriptor[] Assets)
    {
        public static AssetManifestDescriptor CreateFromFile(string file)
        {
            Logger.Trace<AssetManifestDescriptor>($"Loading assets manifest from {file}");
            var timer = Stopwatch.StartNew();
            var buffer = File.ReadAllBytes(FileSystem.GetFullPath(file));
            var manifest = Json.Deserialize<AssetManifestDescriptor>(buffer);
            timer.Stop();
            Logger.Trace<AssetManifestDescriptor>($"Manifest deserialized in {timer.Elapsed}");
            Logger.Trace<AssetManifestDescriptor>($"Manifest contains {manifest.Assets.Length} assets");
            return manifest;
        }
    }
    
}
