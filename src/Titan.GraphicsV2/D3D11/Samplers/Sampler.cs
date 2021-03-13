using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11.Samplers
{
    internal unsafe struct Sampler
    {
        internal ID3D11SamplerState* SamplerState;

        internal TextureFilter Filter;
        internal TextureAddressMode AddressU;
        internal TextureAddressMode AddressV;
        internal TextureAddressMode AddressW;
        internal ComparisonFunc ComparisonFunc;

        internal int Handle;
    }
}
