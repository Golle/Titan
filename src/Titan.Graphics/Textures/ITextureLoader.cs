using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Textures
{

    public record Texture
    {
        public Texture2D Texture2D;
        public ShaderResourceView ResourceView;
    }

    public interface ITextureLoader
    {
        Texture LoadTexture(string filename);
    }


    internal interface IImageLoader
    {
        ImageResource LoadImage(string filename);
    }

    internal record ImageResource
    {

    }


    internal interface IImageDecoder : IDisposable
    {
        uint Width { get; }
        uint Height { get; }
        uint BitsPerPixel { get; }
        uint ImageSize { get; }
        uint Stride { get; }
        public Guid PixelFormat { get; }
        void CopyPixels(IntPtr buffer, uint size);
        unsafe void CopyPixels(byte* buffer, uint size);
    }
}
