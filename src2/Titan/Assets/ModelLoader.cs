using System.Linq;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public class VertexShaderLoader : IAssetLoader
    {
        public string Type => "vertexshader";

        public int OnLoad(in MemoryChunk<byte>[] buffers)
        {
            Logger.Info<VertexShaderLoader>($"OnLoad {buffers.FirstOrDefault().Size}");
            return 10;
        }

        public void OnRelease(int handle)
        {
            Logger.Info<VertexShaderLoader>($"OnRelease {handle}");
        }

        public void Dispose()
        {
            Logger.Info<VertexShaderLoader>("Dispose");
        }
    }

    public class PixelShaderLoader : IAssetLoader
    {
        public string Type => "pixelshader";

        public int OnLoad(in MemoryChunk<byte>[] buffers)
        {
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

    public class ShaderLoader : IAssetLoader
    {
        public string Type => "shader";

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

    public class MaterialsLoader : IAssetLoader
    {
        public string Type => "material";

        public int OnLoad(in MemoryChunk<byte>[] buffers)
        {
            Logger.Info<MaterialsLoader>($"OnLoad {buffers.FirstOrDefault().Size}");
            return 10;
        }

        public void OnRelease(int handle)
        {
            Logger.Info<MaterialsLoader>($"OnRelease {handle}");
        }

        public void Dispose()
        {
            Logger.Info<MaterialsLoader>("Dispose");
        }
    }

    public class ModelLoader : IAssetLoader
    {
        public string Type => "model";

        public int OnLoad(in MemoryChunk<byte>[] buffers)
        {
            Logger.Info<ModelLoader>($"OnLoad {buffers.FirstOrDefault().Size}");
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
