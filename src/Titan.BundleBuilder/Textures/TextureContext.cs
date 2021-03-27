using Titan.BundleBuilder.Models;

namespace Titan.BundleBuilder.Textures
{
    internal record TextureContext(TextureSpecification TextureSpecification)
    {
        internal byte[] Data { get; init; }
    }
}
