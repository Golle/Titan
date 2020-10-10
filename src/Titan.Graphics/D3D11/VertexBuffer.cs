using System;
using System.ComponentModel;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class VertexBuffer<T> : IDisposable where T : unmanaged
    {
        private ComPtr<ID3D11Buffer> _buffer;

        public VertexBuffer(IGraphicsDevice device, uint numberOfVertices)
        {
            D3D11_BUFFER_DESC desc;
            desc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER;
            desc.ByteWidth = (uint) sizeof(T);
            desc.StructureByteStride = (uint)(sizeof(T) * numberOfVertices);
            desc.CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED;
            desc.MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED;
            desc.Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;

            var result = device.Ptr->CreateBuffer(&desc, null, _buffer.GetAddressOf());
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
