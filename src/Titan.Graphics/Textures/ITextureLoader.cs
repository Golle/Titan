namespace Titan.Graphics.Textures
{
    public interface ITextureLoader
    {
        Texture Load(string filename);
        void Release(in Texture texture);
    }
}
