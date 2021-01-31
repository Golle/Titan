using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11.Shaders
{
    internal readonly unsafe struct ShaderResourceView
    {
        public readonly ID3D11ShaderResourceView *ResourceView;
        public readonly ID3D11Resource* Resource;
        public ShaderResourceView(ID3D11ShaderResourceView* pResourceView, ID3D11Resource* pResource)
        {
            ResourceView = pResourceView;
            Resource = pResource;
        }

        public void Release() => ResourceView->Release();
    }
}
