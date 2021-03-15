using Titan.BundleBuilder.Meshes;
using Titan.BundleBuilder.Pipeline;
using Titan.BundleBuilder.WavefrontObj;
using Titan.GraphicsV2.Resources.Models;

namespace Titan.BundleBuilder
{
    internal record BundleDescriptor(string Name, ModelDescriptor[] Meshes);
    internal record ModelDescriptor(string Name, string Filename);

    internal record ModelContext
    {
        internal string AssetsPath { get; init; }
        internal ModelDescriptor ModelDescriptor { get; init; }
        internal WavefrontObject Object { get; init; }
        internal Mesh<VertexData> Mesh { get; set; }
        internal Material[] Materials { get; set; }
    }
}
