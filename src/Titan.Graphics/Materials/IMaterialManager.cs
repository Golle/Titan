using Titan.Graphics.D3D11;
using Titan.Graphics.Textures;

namespace Titan.Graphics.Materials
{
    public interface IMaterialManager
    {
        Material Get(uint handle);
    }

    internal class MaterialManager : IMaterialManager
    {
        private Texture _texture;

        public MaterialManager(ITextureLoader textureLoader)
        {
            _texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\tree01.png");
        }
        public Material Get(uint handle)
        {
            return new Material
            {
                Texture = _texture.ResourceView,
                Ambient = Color.White,
                Diffuse = Color.White,
                Emissive = new Color(0, 0, 0, 0),
                HasNormalMap = false,
                IsTextured = true,
                IsTransparent = false,
                Specular = Color.Blue
            };
        }
    }
}
