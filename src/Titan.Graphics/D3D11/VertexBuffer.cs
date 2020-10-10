using System;
using System.ComponentModel;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class VertexBuffer<T> : IDisposable where T : unmanaged
    {
        internal ID3D11Buffer* Ptr => _buffer.Get();
        
        private ComPtr<ID3D11Buffer> _buffer;

        public VertexBuffer(IGraphicsDevice device, uint numberOfVertices, D3D11_USAGE usage = D3D11_USAGE.D3D11_USAGE_DEFAULT, D3D11_CPU_ACCESS_FLAG cpuAccess = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            D3D11_BUFFER_DESC desc;
            desc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER;
            desc.ByteWidth = (uint) sizeof(T);
            desc.StructureByteStride = (uint)(sizeof(T) * numberOfVertices);
            desc.CpuAccessFlags = cpuAccess;
            desc.MiscFlags = miscFlags;
            desc.Usage = usage;
            InitBuffer(device, desc);
        }

        public VertexBuffer(IGraphicsDevice device, in T[] vertices, D3D11_USAGE usage = D3D11_USAGE.D3D11_USAGE_DEFAULT, D3D11_CPU_ACCESS_FLAG cpuAccess = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            D3D11_BUFFER_DESC desc;
            desc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER;
            desc.ByteWidth = (uint)sizeof(T);
            desc.StructureByteStride = (uint)(sizeof(T) * vertices.Length);
            desc.CpuAccessFlags = cpuAccess;
            desc.MiscFlags = miscFlags;
            desc.Usage = usage;

            D3D11_SUBRESOURCE_DATA data;
            fixed (void* dataPtr = vertices)
            {
                data.pSysMem = dataPtr;
            }

            InitBuffer(device, desc, &data);
        }

        public VertexBuffer(IGraphicsDevice device, void* vertices, uint count, D3D11_USAGE usage = D3D11_USAGE.D3D11_USAGE_DEFAULT, D3D11_CPU_ACCESS_FLAG cpuAccess = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            D3D11_BUFFER_DESC desc;
            desc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER;
            desc.ByteWidth = (uint)sizeof(T);
            desc.StructureByteStride = (uint)(sizeof(T) * count);
            desc.CpuAccessFlags = cpuAccess;
            desc.MiscFlags = miscFlags;
            desc.Usage = usage;

            var data = new D3D11_SUBRESOURCE_DATA
            {
                pSysMem = vertices
            };

            InitBuffer(device, desc, &data);
        }


        private void InitBuffer(IGraphicsDevice device, D3D11_BUFFER_DESC desc, D3D11_SUBRESOURCE_DATA* data = null)
        {
            var result = device.Ptr->CreateBuffer(&desc, data, _buffer.GetAddressOf());
            if (FAILED(result))
            {
                throw new Win32Exception(result, $"Call to CreateBuffer failed with HRESULT {result}");
            }
        }
        
        public void Dispose()
        {
            _buffer.Dispose();
        }
    }
}
