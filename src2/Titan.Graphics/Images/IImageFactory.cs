using System;

namespace Titan.Graphics.Images
{
    public interface IImageLoader
    {
        Image Load(string identifier);
        unsafe Image Load(byte* buffer, uint size);
        Image Load(ReadOnlySpan<byte> buffer);
    }
}
