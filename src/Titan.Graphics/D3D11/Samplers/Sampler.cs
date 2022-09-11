using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D11;

namespace Titan.Graphics.D3D11.Samplers
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct Sampler
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
