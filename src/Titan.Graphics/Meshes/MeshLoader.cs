using System;
using System.IO;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Meshes
{
    internal class MeshLoader : IMeshLoader
    {
        private readonly IGraphicsDevice _device;
        private readonly IVertexBufferManager _vertexBufferManager;

        public MeshLoader(IGraphicsDevice device, IVertexBufferManager vertexBufferManager)
        {
            _device = device;
            _vertexBufferManager = vertexBufferManager;
        }

        public Mesh LoadMesh(string filename)
        {
            IIndexBuffer indexBuffer;
            VertexBufferHandle vertexBuffer;

            using var reader = new ByteReader(File.OpenRead(filename));
            reader.Read<Header>(out var header);
            
            var submeshes = new SubMesh[header.SubMeshCount];
            reader.Read(ref submeshes);

            var vertices = Marshal.AllocHGlobal((int) (header.VertexCount * header.VertexSize)); // TODO: only do a single allocation for both vertices+indices
            try
            {
                unsafe
                {
                    var pVertices = vertices.ToPointer();
                    reader.Read<Vertex>(pVertices, header.VertexCount);
                    vertexBuffer = _vertexBufferManager.CreateVertexBuffer(header.VertexCount, header.VertexSize, pVertices);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(vertices);
            }
            
            var indices = Marshal.AllocHGlobal((int) (header.IndexCount * header.IndexSize));
            try
            {
                unsafe
                {
                    indexBuffer = header.IndexSize switch
                    {
                        2 => CreateIndexBuffer<ushort>(reader, indices.ToPointer(), header.IndexCount),
                        4 => CreateIndexBuffer<uint>(reader, indices.ToPointer(), header.IndexCount),
                        _ => throw new NotSupportedException($"Index size {header.IndexSize} is not supported.")
                    };
                }
            }
            finally
            {
                Marshal.FreeHGlobal(indices);
            }
            return new Mesh(vertexBuffer, indexBuffer, submeshes);
        }

        private unsafe IIndexBuffer CreateIndexBuffer<T>(ByteReader reader, void* pIndices, uint count) where T : unmanaged
        {
            reader.Read<T>(pIndices, count);
            return new IndexBuffer<T>(_device, (T*) pIndices, count);
        }
    }
}
