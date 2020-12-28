using System;
using System.IO;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Meshes
{
    internal class MeshLoader : IMeshLoader
    {
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IIndexBufferManager _indexBufferManager;

        public MeshLoader(IVertexBufferManager vertexBufferManager, IIndexBufferManager indexBufferManager)
        {
            _vertexBufferManager = vertexBufferManager;
            _indexBufferManager = indexBufferManager;
        }
        
        public Mesh[] LoadMesh(string filename)
        {
            using var reader = new ByteReader(File.OpenRead(filename));
            reader.Read<Header>(out var header);

            var meshes = new Mesh[header.NumberOfChunks];
            for (var i = 0; i < header.NumberOfChunks; ++i)
            {
                reader.Read<ChunkHeader>(out var chunkHeader);
                meshes[i] = CreateMesh(chunkHeader, reader);
            }
            return meshes;
        }

        private unsafe Mesh CreateMesh(ChunkHeader chunkHeader, ByteReader reader)
        {
            Handle<IndexBuffer> indexBuffer;
            Handle<VertexBuffer> vertexBuffer;
            var submeshes = new SubMesh[chunkHeader.SubMeshCount];
            reader.Read(ref submeshes);

            var vertices = Marshal.AllocHGlobal((int) (chunkHeader.VertexCount * chunkHeader.VertexSize)); // TODO: only do a single allocation for both vertices+indices
            try
            {
                var pVertices = vertices.ToPointer();
                reader.Read(pVertices, chunkHeader.VertexCount * chunkHeader.VertexSize);
                vertexBuffer = _vertexBufferManager.CreateVertexBuffer(chunkHeader.VertexCount, chunkHeader.VertexSize, pVertices);
            }
            finally
            {
                Marshal.FreeHGlobal(vertices);
            }

            var indices = Marshal.AllocHGlobal((int) (chunkHeader.IndexCount * chunkHeader.IndexSize));
            try
            {
                indexBuffer = chunkHeader.IndexSize switch
                {
                    2 => CreateIndexBuffer<ushort>(reader, indices.ToPointer(), chunkHeader.IndexCount),
                    4 => CreateIndexBuffer<uint>(reader, indices.ToPointer(), chunkHeader.IndexCount),
                    _ => throw new NotSupportedException($"Index size {chunkHeader.IndexSize} is not supported.")
                };
            }
            finally
            {
                Marshal.FreeHGlobal(indices);
            }

            return new Mesh(vertexBuffer, indexBuffer, submeshes);
        }

        private unsafe Handle<IndexBuffer> CreateIndexBuffer<T>(ByteReader reader, void* pIndices, uint count) where T : unmanaged
        {
            reader.Read<T>(pIndices, count);
            return _indexBufferManager.CreateIndexBuffer<T>(count, pIndices);
        }
    }
}
