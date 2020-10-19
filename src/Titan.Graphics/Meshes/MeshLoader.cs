using System;
using System.IO;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;

namespace Titan.Graphics.Meshes
{
    internal class MeshLoader : IMeshLoader
    {
        private readonly IGraphicsDevice _device;

        public MeshLoader(IGraphicsDevice device)
        {
            _device = device;
        }
        public Mesh LoadMesh(string filename)
        {
            IIndexBuffer indexBuffer;
            IVertexBuffer vertexBuffer;

            using var reader = new ByteReader(File.OpenRead(filename));
            reader.Read<Header>(out var header);
            
            var submeshes = new SubMesh[header.SubMeshCount];
            reader.Read(ref submeshes);

            var vertices = Marshal.AllocHGlobal(header.VertexCount * header.VertexSize);
            try
            {
                unsafe
                {
                    var pVertices = vertices.ToPointer();
                    reader.Read<Vertex>(pVertices, header.VertexCount);
                    vertexBuffer = new VertexBuffer<Vertex>(_device, pVertices, (uint)header.VertexCount);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(vertices);
            }
            
            var indices = Marshal.AllocHGlobal(header.IndexCount * header.IndexSize);
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

        private unsafe IIndexBuffer CreateIndexBuffer<T>(ByteReader reader, void* pIndices, int count) where T : unmanaged
        {
            reader.Read<T>(pIndices, count);
            return new IndexBuffer<T>(_device, (T*) pIndices, (uint)count);
        }
    }
}
