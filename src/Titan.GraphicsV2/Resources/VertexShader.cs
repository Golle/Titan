using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Resources
{
    public class VertexShader : IDisposable
    {
        private ComPtr<ID3D11InputLayout> _inputLayout;
        private ComPtr<ID3D11VertexShader> _shader;
        internal unsafe VertexShader(ID3D11InputLayout* inputLayout, ID3D11VertexShader* shader)
        {
            _inputLayout = new ComPtr<ID3D11InputLayout>(inputLayout);
            _shader = new ComPtr<ID3D11VertexShader>(shader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe ID3D11InputLayout* InputLayout() => _inputLayout.Get();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe ID3D11VertexShader* Pointer() => _shader.Get();

        public void Dispose()
        {
            _inputLayout.Dispose();
            _shader.Dispose();
        }
    }
}
