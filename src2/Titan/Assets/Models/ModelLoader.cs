using System;
using System.Runtime.CompilerServices;
using Titan.Assets.Database;
using Titan.Assets.Materials;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Windows.D3D11;

namespace Titan.Assets.Models
{

    public struct Submesh
    {
        public uint StartIndex;
        public uint Count;
        public Handle<Material> Material;
    }
    public class ModelLoader : IAssetLoader
    {
        public unsafe object OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
        {
            var buffer = buffers[0].AsPointer();
            var desc = (MeshDescriptor*) buffer;
            var pVertices = (byte*)(desc + 1);
            var verticesSize = desc->NumberOfVertices * desc->VertexSize;
            var pIndices = pVertices + verticesSize;
            var indicesSize = desc->NumberOfIndices * sizeof(int);
            var submeshDescriptors = new ReadOnlySpan<SubmeshDescriptor>(pIndices + indicesSize, desc->NumberOfSubmeshes);
            var vertexBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                Count = desc->NumberOfVertices,
                Stride = desc->VertexSize,
                InitialData = new DataBlob(pVertices, verticesSize),
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFIED,
                Type = BufferTypes.VertexBuffer,
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
            });

            var indexBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Count = desc->NumberOfIndices,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFIED,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                InitialData = new DataBlob(pIndices, indicesSize),
                Stride = sizeof(int),
                Type = BufferTypes.IndexBuffer,
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
            });

            var submeshes = new Submesh[submeshDescriptors.Length];
            for (var i = 0; i < submeshDescriptors.Length; ++i)
            {
                ref readonly var submesh = ref submeshDescriptors[i];
                submeshes[i] = new Submesh
                {
                    Count = submesh.Count,
                    Material = Unsafe.Unbox<Handle<Material>>(dependencies[submesh.MaterialIndex].Asset) , // TODO: this will fail if a model depends on anything else than a material
                    StartIndex = submesh.StartIndex
                };
            }
            
            return new Model(new Mesh
            {
                IndexBuffer = indexBuffer, 
                VertexBuffer = vertexBuffer,
                Submeshes = submeshes
            });
        }

        public void OnRelease(object asset)
        {
            var model = (Model) asset;
            GraphicsDevice.BufferManager.Release(model.Mesh.VertexBuffer);
            GraphicsDevice.BufferManager.Release(model.Mesh.IndexBuffer);
        }

        public void Dispose()
        {
            Logger.Info<ModelLoader>("Dispose");
        }
    }
}
