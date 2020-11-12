using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Textures
{
    internal interface IImage : IDisposable
    {
        DXGI_FORMAT Format { get; }
        uint Stride { get; }
        uint Width { get; }
        uint Height { get; }
        unsafe byte* GetBuffer();
        uint GetBufferSize();
    }
}
