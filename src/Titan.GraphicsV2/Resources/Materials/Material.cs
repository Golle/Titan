using Titan.Core.Common;
using Titan.GraphicsV2.D3D11.Textures;

namespace Titan.GraphicsV2.Resources.Materials
{
    internal struct Material
    {
        internal string Name;
        internal Handle<Texture> DiffuseTexture;
        // Handle<ShaderProgram> a pre-defined shader program used for this type of material
        internal MaterialProperties Properties;
    }
}
