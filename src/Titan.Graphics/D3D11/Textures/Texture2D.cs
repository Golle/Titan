using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11.Textures
{
    internal readonly unsafe struct Texture2D
    {
        internal readonly ID3D11Texture2D* Resource;
        internal readonly uint Width;
        internal readonly uint Height;
        internal readonly DXGI_FORMAT Format;
        internal Texture2D(ID3D11Texture2D* resource, uint width, uint height, DXGI_FORMAT format)
        {
            Resource = resource;
            Width = width;
            Height = height;
            Format = format;
        }
        public void Release() => Resource->Release();
    }
}
