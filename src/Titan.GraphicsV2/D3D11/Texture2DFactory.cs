using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    public unsafe class Texture2DFactory
    {
        private readonly Device _device;

        internal Texture2DFactory(Device device)
        {
            _device = device;
        }

        public Texture2D Create(uint width, uint height, DXGI_FORMAT format, void* buffer = null, uint stride = 0, D3D11_BIND_FLAG bindFlag = default)
        {
            var desc = new D3D11_TEXTURE2D_DESC
            {
                Height = height,
                Width = width,
                Format = format,
                BindFlags = bindFlag,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED,
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
            return new(texture2D, format, width, height);
        }

    }
}
