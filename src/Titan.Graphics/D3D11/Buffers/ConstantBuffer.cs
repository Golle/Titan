using System;
using System.ComponentModel;
using System.Diagnostics;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11.Buffers
{
    public unsafe class ConstantBuffer<T> : IDisposable where T : unmanaged
    {
        private ComPtr<ID3D11Buffer> _buffer;

        internal ID3D11Buffer* Ptr => _buffer.Get();
        public ConstantBuffer(IGraphicsDevice device, D3D11_USAGE usage = D3D11_USAGE.D3D11_USAGE_DEFAULT, D3D11_CPU_ACCESS_FLAG cpuAccess = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            Debug.Assert(sizeof(T) % 16 == 0, "ConstantBuffer must be 16 byte aligned");

            var size = (uint)sizeof(T);
            var desc = new D3D11_BUFFER_DESC
            {
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                ByteWidth = size,
                StructureByteStride = size,
                CpuAccessFlags = cpuAccess,
                MiscFlags = miscFlags,
                Usage = usage,
            };

            InitBuffer(device.Ptr, &desc);
        }

        public ConstantBuffer(IGraphicsDevice device, in T data, D3D11_USAGE usage = D3D11_USAGE.D3D11_USAGE_DEFAULT, D3D11_CPU_ACCESS_FLAG cpuAccess = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            Debug.Assert(sizeof(T) % 16 == 0, "ConstantBuffer must be 16 byte aligned");
            var size = (uint)sizeof(T);
            var desc = new D3D11_BUFFER_DESC
            {
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                ByteWidth = size,
                StructureByteStride = size,
                CpuAccessFlags = cpuAccess,
                MiscFlags = miscFlags,
                Usage = usage,
            };
            fixed (void* pData = &data)
            {
                var subresourceData = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = pData,
                };
                InitBuffer(device.Ptr, &desc, &subresourceData);
            }
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
