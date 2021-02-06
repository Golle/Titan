using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal readonly unsafe struct VertexShader
    {
        private readonly ID3D11VertexShader* _shader;
        public VertexShader(ID3D11VertexShader* shader)
        {
            _shader = shader;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11VertexShader*(in VertexShader shader) => shader._shader;
        internal void Release() => _shader->Release();
    }
}
