using Titan.Assets.Shaders;

namespace Titan.Assets.Materials
{
    public class Material
    {
        private readonly MaterialProperties _properties;
        public ShaderProgram Shader { get; }
        public ref readonly MaterialProperties Properties => ref _properties;
        public Material(in ShaderProgram shader, in MaterialProperties properties)
        {
            Shader = shader;
            _properties = properties;
        }
    }
}
