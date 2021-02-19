using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11.Textures
{
    internal unsafe class Texture2DFactory : IDisposable
    {
        private ComPtr<ID3D11Device> _device;
        public Texture2DFactory(D3D11GraphicsDevice device)
        {
            _device = new ComPtr<ID3D11Device>(device.Ptr);
        }
        
        public Texture2D Create(uint width, uint height, DXGI_FORMAT format, void * buffer = null, uint stride = 0, D3D11_BIND_FLAG bindFlag = default)
        {
            var desc = new D3D11_TEXTURE2D_DESC
            {
                Height = height,
                Width = width,
                Format = format,
                BindFlags = bindFlag,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFIED,
                Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                ArraySize = 1,
                MipLevels = 1,
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                }
            };
            
            ID3D11Texture2D* texture2D;
            if (buffer != null && stride > 0)
            {
                var data = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = buffer,
                    SysMemPitch = stride
                };
                Common.CheckAndThrow(_device.Get()->CreateTexture2D(&desc, &data, &texture2D), nameof(ID3D11Device.CreateTexture2D));
            }
            else
            {
                Common.CheckAndThrow(_device.Get()->CreateTexture2D(&desc, null, &texture2D), nameof(ID3D11Device.CreateTexture2D));
            }
            return new(texture2D, width, height, format);
        }


        public void Dispose()
        {
            _device.Dispose();
        }
    }
}
