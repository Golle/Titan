using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public class ModelLoader : IAssetLoader
    {
        public string Type => "model";

        public int OnLoad(in MemoryChunk<byte> buffer)
        {
            Logger.Info<ModelLoader>($"OnLoad {buffer.Size}");
            return 10;
        }

        public void OnRelease(int handle)
        {
            Logger.Info<ModelLoader>($"OnRelease {handle}");
        }

        public void Dispose()
        {
            Logger.Info<ModelLoader>("Dispose");
        }
    }
}
