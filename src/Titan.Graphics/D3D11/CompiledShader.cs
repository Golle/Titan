using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public unsafe class CompiledShader : IDisposable
    {
        public nuint BufferSize => _shader.Get()->GetBufferSize();
        public void* Buffer => _shader.Get()->GetBufferPointer();

        private ComPtr<ID3DBlob> _shader;
        public CompiledShader(ComPtr<ID3DBlob> shader)
        {
            _shader = new ComPtr<ID3DBlob>(shader);
        }

        public void Dispose() => _shader.Dispose();
    }
}
