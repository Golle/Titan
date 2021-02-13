using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal readonly unsafe struct PixelShader
    {
        private readonly ID3D11PixelShader* _shader;
        public PixelShader(ID3D11PixelShader* shader)
        {
            _shader = shader;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11PixelShader*(in PixelShader shader) => shader._shader;
        internal void Release() => _shader->Release();
    }
}
