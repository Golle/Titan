using Titan.BundleBuilder.Models.Meshes;
using Titan.BundleBuilder.Models.Pipeline;
using Titan.BundleBuilder.WavefrontObj;
using Titan.GraphicsV2.Resources.Models;

namespace Titan.BundleBuilder.Models
{
    internal record BundleSpecification(string Name, ModelSpecification[] Models, TextureSpecification[] Textures);
    internal record ModelSpecification(string Name, string Filename);
    internal record TextureSpecification(string Name, string Filename);

    internal record ModelContext(ModelSpecification ModelSpecification)
    {
        internal WavefrontObject Object { get; init; }
        internal Mesh<VertexData> Mesh { get; set; }
        internal Material[] Materials { get; set; }
    }
}
