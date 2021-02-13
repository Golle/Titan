using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Resources
{
    public class PixelShader : IDisposable
    {
        private ComPtr<ID3D11PixelShader> _shader;
        internal unsafe PixelShader(ID3D11PixelShader* shader)
        {
            _shader = new ComPtr<ID3D11PixelShader>(shader);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe ID3D11PixelShader* Pointer() => _shader.Get();
        
        public void Dispose()
        {
            _shader.Dispose();
        }
    }
}
