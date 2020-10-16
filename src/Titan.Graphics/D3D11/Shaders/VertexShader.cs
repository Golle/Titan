using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11.Shaders
{
    public unsafe class VertexShader : IDisposable
    {
        internal ref readonly ComPtr<ID3D11VertexShader> Ptr => ref _vertexShader;
        private ComPtr<ID3D11VertexShader> _vertexShader;

        public VertexShader(IGraphicsDevice device, CompiledShader vertexShader)
        {
            CheckAndThrow(device.Ptr->CreateVertexShader(vertexShader.Buffer, vertexShader.BufferSize, null, _vertexShader.GetAddressOf()), "CreateVertexShader");
        }

        public void Dispose() => _vertexShader.Dispose();
    }
}
