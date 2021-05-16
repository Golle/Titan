using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Assets.Database;
using Titan.Assets.Shaders;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Serialization;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Assets.Materials
{
    public class MaterialsLoader : IAssetLoader
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
        {
            Debug.Assert(buffers.Length == 1, "Only a single file can be used for materials");
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

            var material = Json.Deserialize<MatTest>(buffers[0].AsSpan());
            properties.DiffuseColor = material.DiffuseColor;

            Logger.Warning<MaterialsLoader>("Materials have not been fully implemented yet.");

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
