using System;
using System.ComponentModel;
using System.Diagnostics;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.Graphics.D3D11.Buffers
{
    public unsafe class IndexBuffer<T> : IDisposable where T : unmanaged
    {
        public DXGI_FORMAT Format { get; } = typeof(T) == typeof(ushort) ? DXGI_FORMAT.DXGI_FORMAT_R16_UINT : DXGI_FORMAT.DXGI_FORMAT_R32_UINT;
        internal ref readonly ComPtr<ID3D11Buffer> Buffer => ref _buffer;
        
        private ComPtr<ID3D11Buffer> _buffer;
        public IndexBuffer(IGraphicsDevice device, uint count, D3D11_USAGE usage = D3D11_USAGE.D3D11_USAGE_DEFAULT, D3D11_CPU_ACCESS_FLAG cpuAccess = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            Debug.Assert(typeof(T) == typeof(ushort) || typeof(T) == typeof(uint), "Currently supported formats are ushort and uint");
            Debug.Assert(count > 0, "Must be atleast 1 index");
            var desc = new D3D11_BUFFER_DESC
            {
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
                ByteWidth = (uint) (sizeof(T) * count),
                StructureByteStride = (uint)sizeof(T),
                CpuAccessFlags = cpuAccess,
                MiscFlags = miscFlags,
                Usage = usage,
            };

            InitBuffer(device.Ptr, &desc);
        }

        public IndexBuffer(IGraphicsDevice device, in T[] indices, D3D11_USAGE usage = D3D11_USAGE.D3D11_USAGE_DEFAULT, D3D11_CPU_ACCESS_FLAG cpuAccess = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            Debug.Assert(typeof(T) == typeof(ushort) || typeof(T) == typeof(uint), "Currently supported formats are ushort and uint");
            Debug.Assert(indices != null, "Indices can't be null");

            var desc = new D3D11_BUFFER_DESC
            {
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
                StructureByteStride = (uint)sizeof(T),
                ByteWidth = (uint)(sizeof(T) * indices.Length),
                CpuAccessFlags = cpuAccess,
                MiscFlags = miscFlags,
                Usage = usage
            };

            fixed (void* pIndices = indices)
            {
                var data = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = pIndices
                };
                InitBuffer(device.Ptr, &desc, &data);
            }
        }

        public IndexBuffer(IGraphicsDevice device, T* indices, uint count, D3D11_USAGE usage = D3D11_USAGE.D3D11_USAGE_DEFAULT, D3D11_CPU_ACCESS_FLAG cpuAccess = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            Debug.Assert(typeof(T) == typeof(ushort) || typeof(T) == typeof(uint), "Currently supported formats are ushort and uint");
            Debug.Assert(indices != null, "Indices can't be null");
            Debug.Assert(count > 0, "Must be atleast 1 index in the data");

            var desc = new D3D11_BUFFER_DESC
            {
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
                StructureByteStride = (uint)sizeof(T),
                ByteWidth = (uint)(sizeof(T) * count),
                CpuAccessFlags = cpuAccess,
                MiscFlags = miscFlags,
                Usage = usage
            };

            var data = new D3D11_SUBRESOURCE_DATA
            {
                pSysMem = indices
            };
            InitBuffer(device.Ptr, &desc, &data);
        }

        private void InitBuffer(ID3D11Device* device, D3D11_BUFFER_DESC* desc, D3D11_SUBRESOURCE_DATA* data = null)
        {
            var result = device->CreateBuffer(desc, data, _buffer.GetAddressOf());
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
