using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Graphics.Resources;

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
            IndexBufferHandle indexBuffer;
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

                    reader.Read(pVertices, header.VertexCount * header.VertexSize);

                    //reader.Read<Vertex>(pVertices, header.VertexCount);
                    vertexBuffer = _device.VertexBufferManager.CreateVertexBuffer(header.VertexCount, header.VertexSize, pVertices);
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
            return new Mesh(header.VertexSize == 56, vertexBuffer, indexBuffer, submeshes);
        }

        private unsafe IndexBufferHandle CreateIndexBuffer<T>(ByteReader reader, void* pIndices, uint count) where T : unmanaged
        {
            reader.Read<T>(pIndices, count);
            return _device.IndexBufferManager.CreateIndexBuffer<T>(count, pIndices);
        }
    }
}
