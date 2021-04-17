using System.Diagnostics;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Serialization;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Assets
{
    public class VertexShaderLoader : IAssetLoader
    {
        public void Init(AssetsManager assetsManager) { }
        public int OnLoad(in MemoryChunk<byte>[] buffers)
        {
            Debug.Assert(buffers.Length == 2, $"{nameof(VertexShaderLoader)} must have 2 files");

            var inputLayoutDescription = Json.Deserialize<InputLayoutDescription[]>(buffers[1].AsSpan());
            var vertexShader = GraphicsDevice
                .ShaderManager
                .CreateVertexShader(new VertexShaderCreation(buffers[0], "main", "vs_5_0", inputLayoutDescription));
            Logger.Trace<VertexShaderLoader>($"Handle: {vertexShader}");
            return vertexShader;
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
}
