namespace Titan.Graphics.Textures
{
    public interface ITextureLoader
    {
        Texture LoadTexture(string filename);
        void UnloadTexture(Texture texture);
    }
}
