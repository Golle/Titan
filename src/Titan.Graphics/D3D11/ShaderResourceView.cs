using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class ShaderResourceView : IDisposable
    {
        public ref readonly ComPtr<ID3D11ShaderResourceView> Ptr => ref _resource;
        private ComPtr<ID3D11ShaderResourceView> _resource;

        public ShaderResourceView(IGraphicsDevice device, IResource resource, DXGI_FORMAT format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN)
        {
            D3D11_SHADER_RESOURCE_VIEW_DESC desc = default;
            desc.Format = format == DXGI_FORMAT.DXGI_FORMAT_UNKNOWN ? resource.Format : format;
            desc.ViewDimension = D3D_SRV_DIMENSION.D3D11_SRV_DIMENSION_TEXTURE2D;
            desc.Texture2D.MipLevels = 1;
            desc.Texture2D.MostDetailedMip = 0;
            CheckAndThrow(device.Ptr->CreateShaderResourceView(resource.AsResourcePointer(), &desc, _resource.GetAddressOf()), "CreateShaderResourceView");
        }

        public void Dispose() => _resource.Dispose();
    }
}
