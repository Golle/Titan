using System.Diagnostics;
using System.Linq;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public class PixelShaderLoader : IAssetLoader
    {
        public int OnLoad(in MemoryChunk<byte>[] buffers)
        {
            Debug.Assert(buffers.Length == 1, $"{nameof(PixelShaderLoader)} only supports single files");
            Logger.Info<PixelShaderLoader>($"OnLoad {buffers.FirstOrDefault().Size}");
            return 10;
        }

        public void OnRelease(int handle)
        {
            Logger.Info<PixelShaderLoader>($"OnRelease {handle}");
        }

        public void Dispose()
        {
            Logger.Info<PixelShaderLoader>("Dispose");
        }
    }
}
