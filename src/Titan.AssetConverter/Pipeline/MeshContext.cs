using Titan.AssetConverter.Exporter;
using Titan.AssetConverter.Pipeline.Middlewares;
using Titan.AssetConverter.WavefrontObj;
using Titan.GraphicsV2.Resources;

namespace Titan.AssetConverter.Pipeline
{
    internal record MeshContext
    {
        public string Filename { get; init; }
        public WavefrontObject Object { get; init; }
        public Mesh<VertexData> Mesh { get; init; }
        public string OutputFolder { get; init; }
        public string Name { get; init; }
        public Material[] Materials { get; init; }
    }
}
