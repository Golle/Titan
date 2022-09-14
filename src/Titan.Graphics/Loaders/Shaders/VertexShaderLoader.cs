using System.Diagnostics;
using Titan.Assets;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Serialization;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Loaders.Shaders
{
    public class VertexShaderLoader : IAssetLoader
    {
        public int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
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

            GraphicsDevice.ShaderManager.Release((Handle<VertexShader>)handle);
        }

        public void Dispose()
        {
            Logger.Info<VertexShaderLoader>("Dispose");
        }
    }
}
