using System;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Core.IO;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Buffers;
using Titan.Windows.Win32.D3D11;
using Buffer = Titan.GraphicsV2.D3D11.Buffers.Buffer;

namespace Titan.GraphicsV2.Resources
{
    internal class ModelLoader
    {
        private readonly Device _device;
        private readonly IFileReader _fileReader;

        public ModelLoader(Device device, IFileReader fileReader)
        {
            _device = device;
            _fileReader = fileReader;
        }


        public Model3D Load(string identifier)
        {
            using var file = new ByteReader(_fileReader.OpenRead(identifier));

            file.Read<MeshDataHeader>(out var header);

            var vertexBuffer = CreateVertexBuffer(header, file);
            var indexBuffer = CreateIndexBuffer(header, file);
            var submeshes = CreateSubMeshes(header, file);


            return new Model3D
            {
                SubMeshes = submeshes,
                IndexBuffer = indexBuffer,
                VertexBuffer = vertexBuffer,
                SubMeshCount = submeshes.Length
            };
        }

        private unsafe SubMesh[] CreateSubMeshes(MeshDataHeader header, ByteReader file)
        {
            if (header.SubMeshes == 0)
            {
                return Array.Empty<SubMesh>();
            }

            var totalSize = header.SubMeshes * header.SubMeshSize;
            var buffer = (SubMeshData*)Marshal.AllocHGlobal(totalSize);
            try
            {
                file.Read<SubMeshData>(buffer, (uint)header.SubMeshes);
                var result = new SubMesh[header.SubMeshes];
                for (var i = 0; i < header.SubMeshes; ++i)
                {
                    ref readonly var submesh = ref buffer[i];
                    result[i] = new SubMesh {Count = submesh.Count, Start = submesh.StartIndex};
                }
                return result;
            }
            finally
            {
                Marshal.FreeHGlobal((nint) buffer);
            }
        }

        private unsafe Handle<Buffer> CreateIndexBuffer(in MeshDataHeader header, ByteReader file)
        {
            var totalSize = header.Indicies * header.IndexSize;
            var buffer = Marshal.AllocHGlobal(totalSize);
            try
            {
                file.Read(buffer.ToPointer(), (uint)totalSize);
                return _device.BufferManager.Create(new BufferCreation
                {
                    Count = (uint)header.Indicies,
                    Type = BufferTypes.IndexBuffer,
                    Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                    InitialData = new DataBlob(buffer.ToPointer()),
                    Stride = (uint)header.IndexSize
                });
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private unsafe Handle<Buffer> CreateVertexBuffer(in MeshDataHeader header, ByteReader file)
        {
            var buffer = Marshal.AllocHGlobal(header.Vertices * header.VertexSize);
            try
            {
                file.Read(buffer.ToPointer(), (uint)(header.Vertices * header.VertexSize));
                return _device.BufferManager.Create(new BufferCreation
                {
                    Count = (uint)header.Vertices,
                    Stride = (uint)header.VertexSize,
                    Type = BufferTypes.VertexBuffer,
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                    InitialData = new DataBlob(buffer.ToPointer()),
                    Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
                });
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
