using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Textures
{




    public record TextureCreation
    {
        // TODO: create new enums for these
        public TextureFormats Format { get; init; }
        public DepthStencilFormats DepthStencilFormat { get; init; }
        public D3D11_USAGE Usage { get; init; }

        public TextureBindFlags Binding { get; init; }
        public uint Width { get; init; }
        public uint Height { get; init; }

        public DataBlob InitialData { get; init; }
        public  uint DataStride { get; init; }
    }
}
