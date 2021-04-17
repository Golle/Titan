using System.Linq;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public class ShaderLoader : IAssetLoader
    {
        public int OnLoad(in MemoryChunk<byte>[] buffers)
        {
            Logger.Info<ShaderLoader>($"OnLoad {buffers?.FirstOrDefault().Size}");
            return 10;
        }

        public void OnRelease(int handle)
        {
            Logger.Info<ShaderLoader>($"OnRelease {handle}");
        }

        public void Dispose()
        {
            Logger.Info<ShaderLoader>("Dispose");
        }
    }
}
