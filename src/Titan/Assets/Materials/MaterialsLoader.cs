using System;
using System.Diagnostics;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Serialization;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Assets.Materials
{
    public class MaterialsLoader : IAssetLoader
    {
        public int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
        {
            Debug.Assert(buffers.Length == 1, "Only a single file can be used for materials");
            Handle<VertexShader> vertexShader = default;
            Handle<PixelShader> pixelShader = default;

            var diffuseMap = Handle<Texture>.Null;
            var ambientMap = Handle<Texture>.Null;
            
            foreach (ref readonly var dependency in dependencies)
            {
                switch (dependency.Type)
                {
                    case AssetTypes.VertexShader:
                        vertexShader = dependency.AssetHandle;
                        break;
                    case AssetTypes.PixelShader:
                        pixelShader = dependency.AssetHandle;
                        break;
                    case AssetTypes.Texture:
                        switch (dependency.Name)
                        {
                            case "diffuse":
                                diffuseMap = dependency.AssetHandle;
                                break;
                            case "ambient":
                                ambientMap = dependency.AssetHandle;
                                break;
                            default:
                                Logger.Error<MaterialsLoader>($"Texture type {dependency.Name} is not recognized.");
                                break;
                        }
                        break;
                }
            }

            Debug.Assert(pixelShader.IsValid(), "The PixelShader handle is not valid");
            Debug.Assert(vertexShader.IsValid(), "The VertexShader handle is not valid");

            var material = Json.Deserialize<MaterialDescriptor>(buffers[0].AsSpan());
            var args = new MaterialCreation
            {
                PixelShader = pixelShader,
                VertexShader = vertexShader,
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

        public void OnRelease(int handle)
        {
            Resources.Material.Release(handle);
        }

        public void Dispose()
        {
            //_manager.Dispose(); TODO: who owns the manager?
            // Nothing to dispose
        }
    }
}
