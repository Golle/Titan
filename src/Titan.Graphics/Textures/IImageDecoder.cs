using System;

namespace Titan.Graphics.Textures
{
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
