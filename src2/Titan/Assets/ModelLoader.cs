using System.Linq;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public class ModelLoader : IAssetLoader
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers)
        {
            Logger.Info<ModelLoader>($"OnLoad {buffers.FirstOrDefault().Size}");
            return 10;
        }

        public void OnRelease(object asset)
        {

            Logger.Info<ModelLoader>($"OnRelease {asset?.GetType().Name}");
        }

        public void Dispose()
        {
            Logger.Info<ModelLoader>("Dispose");
        }
    }
}
