using System;
using Titan.Assets;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Windows.D3D11;

namespace Titan.Graphics.Loaders.Models
{
    public class ModelLoader : IAssetLoader
    {
        private readonly ModelManager _modelManager;
        public ModelLoader(ModelManager modelManager)
        {
            _modelManager = modelManager;
        }

        public unsafe int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
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

            Span<Submesh> submeshes = stackalloc Submesh[submeshDescriptors.Length];
            for (var i = 0; i < submeshDescriptors.Length; ++i)
            {
                ref readonly var submesh = ref submeshDescriptors[i];
                submeshes[i] = new Submesh
                {
                    Count = submesh.Count,
                    Material = dependencies[submesh.MaterialIndex].AssetHandle , // TODO: this will fail if a model depends on anything else than a material
                    StartIndex = submesh.StartIndex
                };
            }
            return _modelManager.Create(new ModelCreation
            {
                Submeshes = submeshes,
                IndexBuffer = indexBuffer, 
                VertexBuffer = vertexBuffer
            });
        }

        public void OnRelease(int handle)
        {
            ref readonly var model = ref _modelManager.Access(handle);
            GraphicsDevice.BufferManager.Release(model.Mesh.VertexBuffer);
            GraphicsDevice.BufferManager.Release(model.Mesh.IndexBuffer);
            
            _modelManager.Release(handle);
        }

        public void Dispose()
        {
            Logger.Info<ModelLoader>("Dispose");
        }
    }
}
