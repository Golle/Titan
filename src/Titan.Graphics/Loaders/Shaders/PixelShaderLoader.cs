using System;
using System.Diagnostics;
using Titan.Assets;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Loaders.Shaders
{
    public class PixelShaderLoader : IAssetLoader
    {
        public int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
        {
            Debug.Assert(buffers.Length == 1, $"{nameof(PixelShaderLoader)} only supports single files");

            return GraphicsDevice.ShaderManager.CreatePixelShader(new PixelShaderCreation(buffers[0], "main", "ps_5_0"));
        }

        public void OnRelease(int handle)
        {
            Logger.Info<PixelShaderLoader>($"OnRelease {handle}");
            GraphicsDevice.ShaderManager.Release((Handle<PixelShader>)handle);
        }

        public void Dispose()
        {
            Logger.Info<PixelShaderLoader>("Dispose");
        }
    }
}
