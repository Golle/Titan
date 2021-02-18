using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe struct TextureCreation
    {
        // TODO: create new enums for these
        internal DXGI_FORMAT Format;
        internal D3D11_USAGE Usage;

        internal TextureBindFlags Binding;
        internal uint Width;
        internal uint Height;
        internal void* InitialData;
        internal uint DataStride;
    }
}
