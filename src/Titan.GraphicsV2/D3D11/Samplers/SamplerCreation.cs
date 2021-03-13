namespace Titan.GraphicsV2.D3D11.Samplers
{
    internal record SamplerCreation
    {
        internal TextureFilter Filter { get; init; }
        internal TextureAddressMode AddressU { get; init; }
        internal TextureAddressMode AddressV { get; init; }
        internal TextureAddressMode AddressW { get; init; }
        internal ComparisonFunc ComparisonFunc { get; init; }

        internal TextureAddressMode AddressAll
        {
            init => AddressU = AddressV = AddressW = value;
        }
    }
}
