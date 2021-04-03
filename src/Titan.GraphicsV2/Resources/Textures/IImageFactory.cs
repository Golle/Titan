namespace Titan.GraphicsV2.Resources.Textures
{
    public interface IImageLoader
    {
        Image Load(string identifier);
        unsafe Image Load(byte* buffer, uint size);
    }
}
