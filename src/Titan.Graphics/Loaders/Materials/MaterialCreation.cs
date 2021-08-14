using Titan.Core;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.Loaders.Materials
{
    public record MaterialCreation
    {
        public Color AmbientColor { get; init; }
        public Color DiffuseColor { get; init; }
        public Color SpecularColor { get; init; }
        public Color EmissiveColor { get; init; }
        public Handle<Texture> DiffuseMap { get; init; }
        public Handle<Texture> AmbientMap { get; init; }
        public Handle<VertexShader> VertexShader { get; init; }
        public Handle<PixelShader> PixelShader { get; init; }
    }
}
