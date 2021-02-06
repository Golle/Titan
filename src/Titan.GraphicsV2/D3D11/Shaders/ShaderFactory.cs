using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal unsafe class ShaderFactory
    {
        private readonly Device _device;

        public ShaderFactory(Device device)
        {
            _device = device;
        }

        internal VertexShader CreateVertexShader(in CompiledShader compiledShader)
        {
            ID3D11VertexShader* vertexShader;
            CheckAndThrow(_device.Get()->CreateVertexShader(compiledShader.GetBuffer(), compiledShader.GetSize(), null, &vertexShader), nameof(ID3D11Device.CreateVertexShader));
            return new (vertexShader);
        }

        internal PixelShader CreatePixelShader(in CompiledShader compiledShader)
        {
            ID3D11PixelShader* pixelShader;
            CheckAndThrow(_device.Get()->CreatePixelShader(compiledShader.GetBuffer(), compiledShader.GetSize(), null, &pixelShader), nameof(ID3D11Device.CreatePixelShader));
            return new(pixelShader);
        }
    }
}
