using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11.Buffers
{
    public interface IIndexBuffer : IDisposable
    {
        DXGI_FORMAT Format { get; }
        uint NumberOfIndices { get; }
        internal ref readonly ComPtr<ID3D11Buffer> Buffer { get; }
    }
}
