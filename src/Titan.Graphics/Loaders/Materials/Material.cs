using Titan.Core;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Loaders.Materials
{
    public struct Material
    {
        public MaterialProperties Properties;
        public Handle<VertexShader> VertexShader;
        public Handle<PixelShader> PixelShader;
    }
}
