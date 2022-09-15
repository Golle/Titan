namespace Titan.Graphics.Images
{
    public interface IImageLoader : IDisposable
    {
        Image Load(string identifier);
        unsafe Image Load(byte* buffer, uint size);
        Image Load(ReadOnlySpan<byte> buffer);
    }
}
