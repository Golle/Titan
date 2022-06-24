using System;
using System.Diagnostics;
using Titan.Assets;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Loaders.Shaders;

public class ComputeShaderLoader : IAssetLoader
{
    public int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
    {
        Debug.Assert(buffers.Length == 1, $"{nameof(ComputeShaderLoader)} only supports single files");

        return GraphicsDevice.ShaderManager.CreateComputeShader(new ComputeShaderCreation(buffers[0], "main", "cs_5_0"));
    }

    public void OnRelease(int handle)
    {
        Logger.Info<ComputeShaderLoader>($"OnRelease {handle}");
        GraphicsDevice.ShaderManager.Release((Handle<ComputeShader>)handle);
    }

    public void Dispose()
    {
        Logger.Info<ComputeShaderLoader>("Dispose");
    }
}
