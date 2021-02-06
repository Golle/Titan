using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal readonly unsafe struct CompiledShader
    {
        private readonly ID3DBlob* _data;
        public CompiledShader(ID3DBlob* data)
        {
            _data = data;
        }

        internal void* GetBuffer() => _data->GetBufferPointer();
        internal nuint GetSize() => _data->GetBufferSize();
        internal void Release() => _data->Release();
    }
}
