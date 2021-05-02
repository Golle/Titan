using System;
using System.Diagnostics;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics;

namespace Titan.Assets
{

    public class Material
    {
        private readonly MaterialProperties _properties;
        public ShaderProgram Shader { get; }
        public ref readonly MaterialProperties Properties => ref _properties;
        public Material(in ShaderProgram shader, in MaterialProperties properties)
        {
            Shader = shader;
            _properties = properties;
        }
    }

    public struct MaterialProperties
    {
        public Color DiffuseColor;
    }

    public class MaterialsLoader : IAssetLoader
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<AssetDependency> dependencies)
        {
            ShaderProgram shader = default;
            foreach (var dependency in dependencies)
            {
                if (dependency.Type == AssetTypes.Shader)
                {
                    shader = (ShaderProgram) dependency.Asset;
                }
            }

            Debug.Assert(shader.PixelShader.IsValid(), "The PixelShader handle is not valid");
            Debug.Assert(shader.VertexShader.IsValid(), "The VertexShader handle is not valid");

            return new Material(shader, default);
        }

        public void OnRelease(object asset)
        {
            // No resources allocated
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
