namespace Titan.Graphics.D3D11.Samplers
{
    public record SamplerCreation
    {
        public TextureFilter Filter { get; init; }
        public TextureAddressMode AddressU { get; init; }
        public TextureAddressMode AddressV { get; init; }
        public TextureAddressMode AddressW { get; init; }
        public ComparisonFunc ComparisonFunc { get; init; }

        public TextureAddressMode AddressAll
        {
            init => AddressU = AddressV = AddressW = value;
        }
    }
}
