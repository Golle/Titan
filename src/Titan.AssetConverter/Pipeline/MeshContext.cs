using Titan.AssetConverter.Exporter;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter.Pipeline
{
    internal record MeshContext
    {
        public string Filename { get; init; }
        public WavefrontObject Object { get; init; }
        public Mesh<Vertex> Mesh { get; init; }
        public string OutputFolder { get; init; }
        public string Name { get; init; }
    }
}
