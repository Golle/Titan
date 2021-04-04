using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Textures
{
    public record TextureCreation
    {
        // TODO: create new enums for these
        internal DXGI_FORMAT Format { get; init; }
        internal D3D11_USAGE Usage { get; init; }

        internal TextureBindFlags Binding { get; init; }
        internal uint Width { get; init; }
        internal uint Height { get; init; }

        internal DataBlob InitialData { get; init; }
        internal uint DataStride { get; init; }
    }
}
