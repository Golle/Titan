using Titan.BundleBuilder.Common;
using Titan.BundleBuilder.Models;

namespace Titan.BundleBuilder.Textures
{
    internal record TextureContext(TextureSpecification TextureSpecification)
    {
        internal byte[] Data { get; init; }
        public Image Image { get; init; }
    }
}
