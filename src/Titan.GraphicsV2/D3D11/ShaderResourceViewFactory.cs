using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class ShaderResourceViewFactory
    {
        private readonly Device _device;

        internal ShaderResourceViewFactory(Device device)
        {
            _device = device;
        }

        public ShaderResourceView Create(in Texture2D texture) => Create(texture, texture.Format);
        public ShaderResourceView Create(ID3D11Resource* resource, DXGI_FORMAT format)
        {
            var desc = new D3D11_SHADER_RESOURCE_VIEW_DESC
            {
                Format = format,
                Texture2D = new D3D11_TEX2D_SRV {MipLevels = 1, MostDetailedMip = 0},
                ViewDimension = D3D_SRV_DIMENSION.D3D10_1_SRV_DIMENSION_TEXTURE2D
            };
            ID3D11ShaderResourceView* shaderResourceView;
            Common.CheckAndThrow(_device.Get()->CreateShaderResourceView(resource, &desc, &shaderResourceView), nameof(ID3D11Device.CreateShaderResourceView));
            return new ShaderResourceView(shaderResourceView);
        }

    }
}
