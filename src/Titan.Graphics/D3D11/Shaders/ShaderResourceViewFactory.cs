using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D_SRV_DIMENSION;

namespace Titan.Graphics.D3D11.Shaders
{
    internal unsafe class ShaderResourceViewFactory: IDisposable
    {
        private ComPtr<ID3D11Device> _device;

        public ShaderResourceViewFactory(D3D11GraphicsDevice device)
        {
            _device = new ComPtr<ID3D11Device>(device.Ptr);
        }

        public ShaderResourceView Create(ID3D11Resource * pResource, DXGI_FORMAT format)
        {
            var desc = new D3D11_SHADER_RESOURCE_VIEW_DESC
            {
                Texture2D = new D3D11_TEX2D_SRV
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                },
                Format = format,
                ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D
            };
            ID3D11ShaderResourceView* shaderResourceView;
            CheckAndThrow(_device.Get()->CreateShaderResourceView(pResource,&desc, &shaderResourceView), nameof(ID3D11Device.CreateShaderResourceView));
            return new ShaderResourceView(shaderResourceView, pResource);
        }

        public void Dispose()
        {
            _device.Dispose();
        }
    }
}
