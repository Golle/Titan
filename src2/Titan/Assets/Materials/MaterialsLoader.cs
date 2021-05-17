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

            var diffuseMap = Handle<Texture>.Null;
            var ambientMap = Handle<Texture>.Null;
            
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
                            diffuseMap = Unsafe.Unbox<Handle<Texture>>(dependency.Asset);
                            break;
                        case "ambient":
                            ambientMap = Unsafe.Unbox<Handle<Texture>>(dependency.Asset);
                            break;
                        default:
                            Logger.Error<MaterialsLoader>($"Texture type {dependency.Name} is not recognized.");
                            break;
                    }
                }
            }

            Debug.Assert(shader.PixelShader.IsValid(), "The PixelShader handle is not valid");
            Debug.Assert(shader.VertexShader.IsValid(), "The VertexShader handle is not valid");

            var material = Json.Deserialize<MaterialDescriptor>(buffers[0].AsSpan());
            var args = new MaterialCreation
            {
                Shader = shader,
                DiffuseColor = material.DiffuseColor,
                AmbientColor = material.AmbientColor,
                EmissiveColor =  material.EmissiveColor,
                SpecularColor = material.SpecularColor,
                DiffuseMap = diffuseMap,
                AmbientMap = ambientMap
            };

            
            Logger.Warning<MaterialsLoader>("Materials have not been fully implemented yet.");
            return Resources.Material.Create(args);
        }

        public void OnRelease(object asset)
        {
            var handle = Unsafe.Unbox<Handle<Material>>(asset);
            Resources.Material.Release(handle);
        }

        public void Dispose()
        {
            //_manager.Dispose(); TODO: who owns the manager?
            // Nothing to dispose
        }
    }
}
