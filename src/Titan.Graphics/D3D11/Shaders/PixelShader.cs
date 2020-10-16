using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11.Shaders
{
    public unsafe class PixelShader : IDisposable
    {
        internal ref readonly ComPtr<ID3D11PixelShader> Ptr => ref _pixelShader;
        private ComPtr<ID3D11PixelShader> _pixelShader;
        public PixelShader(IGraphicsDevice device, CompiledShader pixelShader)
        {
            Common.CheckAndThrow(device.Ptr->CreatePixelShader(pixelShader.Buffer, pixelShader.BufferSize, null, _pixelShader.GetAddressOf()), "CreatePixelShader");
        }

        public void Dispose() => _pixelShader.Dispose();
    }
}
