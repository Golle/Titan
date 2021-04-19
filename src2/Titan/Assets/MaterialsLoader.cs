using System;
using System.Linq;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public class MaterialsLoader : IAssetLoader
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<AssetDependency> dependencies)
        {
            Logger.Info<MaterialsLoader>($"OnLoad {buffers.FirstOrDefault().Size}");
            return 10;
        }

        public void OnRelease(object asset)
        {
            Logger.Info<MaterialsLoader>($"OnRelease {asset?.GetType().Name}");
        }

        public void Dispose()
        {
            Logger.Info<MaterialsLoader>("Dispose");
        }
    }
}
