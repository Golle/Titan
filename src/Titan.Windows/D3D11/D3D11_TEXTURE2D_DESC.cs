// ReSharper disable InconsistentNaming

using Titan.Windows.DXGI;

namespace Titan.Windows.D3D11
{
    public struct D3D11_TEXTURE2D_DESC
    {
        public uint Width;
        public uint Height;
        public uint MipLevels;
        public uint ArraySize;
        public DXGI_FORMAT Format;
        public DXGI_SAMPLE_DESC SampleDesc;
        public D3D11_USAGE Usage;
        public D3D11_BIND_FLAG BindFlags;
        public D3D11_CPU_ACCESS_FLAG CpuAccessFlags;
        public D3D11_RESOURCE_MISC_FLAG MiscFlags;
    }
}
