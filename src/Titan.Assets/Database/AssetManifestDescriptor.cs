using System.Diagnostics;
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
            
            using var fileHandle = Core.IO.FileSystem.OpenReadHandle(file);
            var bytes = fileHandle.ReadAllBytes();

            var manifest = Json.Deserialize<AssetManifestDescriptor>(bytes);
            timer.Stop();
            Logger.Trace<AssetManifestDescriptor>($"Manifest deserialized in {timer.Elapsed}");
            Logger.Trace<AssetManifestDescriptor>($"Manifest contains {manifest.Assets.Length} assets");
            return manifest;
        }
    }
    
}
