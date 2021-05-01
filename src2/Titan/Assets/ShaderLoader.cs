using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Assets
{
    internal struct ShaderProgram
    {
        public Handle<VertexShader> VertexShader;
        public Handle<PixelShader> PixelShader;
    }

    public class ShaderLoader : IAssetLoader
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<AssetDependency> dependencies)
        {
            Handle<VertexShader> vertexShaderHandle = default;
            Handle<PixelShader> pixelShaderHandle = default;

            foreach (var dependency in dependencies)
            {
                switch (dependency.Type)
                {
                    case AssetTypes.VertexShader:
                        vertexShaderHandle = Unsafe.Unbox<Handle<VertexShader>>(dependency.Asset);
                        break;
                    case AssetTypes.PixelShader:
                        pixelShaderHandle = Unsafe.Unbox<Handle<PixelShader>>(dependency.Asset);
                        break;
                    default:
                        Logger.Warning<ShaderLoader>($"Unrecognized dependency type {dependency.Type}");
                        break;
                }
            }

            Debug.Assert(vertexShaderHandle.IsValid(), "VertexShader is not valid");
            Debug.Assert(pixelShaderHandle.IsValid(), "PixelShader is not valid");

            return new ShaderProgram
            {
                VertexShader = vertexShaderHandle,
                PixelShader = pixelShaderHandle
            };
        }

        public void OnRelease(object asset)
        {
            // NOOP, this loader doesn't allocate any resources, just uses dependencies.
        }

        public void Dispose()
        {
        }
    }
}
