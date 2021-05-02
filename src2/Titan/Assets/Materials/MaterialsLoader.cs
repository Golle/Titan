using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Assets.Materials
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
        public Handle<Texture> DiffuseMap;
        public Handle<Texture> BumpMap;
    }

    public class MaterialsLoader : IAssetLoader
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
        {
            ShaderProgram shader = default;
            MaterialProperties properties = default;
            foreach (ref readonly var dependency in dependencies)
            {
                if (dependency.Type == AssetTypes.Shader)
                {
                    shader = (ShaderProgram) dependency.Asset;
                }
                else if (dependency.Type == AssetTypes.Texture)
                {
                    switch (dependency.Name)
                    {
                        case "diffuse":
                            properties.DiffuseMap = Unsafe.Unbox<Handle<Texture>>(dependency.Asset);
                            break;
                        case "bumpmap":
                            properties.BumpMap = Unsafe.Unbox<Handle<Texture>>(dependency.Asset);
                            break;
                            default:
                                Logger.Error<MaterialsLoader>($"Texture type {dependency.Name} is not recognized.");
                                break;
                    }
                }
            }

            Debug.Assert(shader.PixelShader.IsValid(), "The PixelShader handle is not valid");
            Debug.Assert(shader.VertexShader.IsValid(), "The VertexShader handle is not valid");

            return new Material(shader, properties);
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
