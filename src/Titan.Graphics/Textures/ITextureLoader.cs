using Titan.Graphics.D3D11;

namespace Titan.Graphics.Textures
{
    public interface ITextureLoader
    {
        Texture2D LoadTexture(string filename);
    }
}
