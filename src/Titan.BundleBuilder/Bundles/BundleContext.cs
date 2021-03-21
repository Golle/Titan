using Titan.BundleBuilder.Models;
using Titan.BundleBuilder.Textures;

namespace Titan.BundleBuilder.Bundles
{
    internal record BundleContext(string Name)
    {
        internal ModelContext[] Models { get; init; }
        internal TextureContext[] Textures { get; init; }
        
        public byte[] DataBlob { get; init; }
        
        public MeshDescriptor[] MeshDescriptors { get; init; }
        public TextureDescriptor[] TextureDescriptors { get; set; }
    }
}
