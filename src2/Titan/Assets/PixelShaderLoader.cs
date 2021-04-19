using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Assets
{
    public class PixelShaderLoader : IAssetLoader
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<AssetDependency> dependencies)
        {
            Debug.Assert(buffers.Length == 1, $"{nameof(PixelShaderLoader)} only supports single files");

            return GraphicsDevice.ShaderManager.CreatePixelShader(new PixelShaderCreation(buffers[0], "main", "ps_5_0"));
        }

        public void OnRelease(object asset)
        {
            var handle = Unsafe.Unbox<Handle<PixelShader>>(asset);
            Logger.Info<PixelShaderLoader>($"OnRelease {handle}");
            GraphicsDevice.ShaderManager.Release(new Handle<PixelShader>(handle));
        }

        public void Dispose()
        {
            Logger.Info<PixelShaderLoader>("Dispose");
        }
    }
}
