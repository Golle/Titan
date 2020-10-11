using System.Drawing.Imaging;

namespace Titan.Core.Images
{
    public record ImageAsset
    {
        public uint Width;
        public uint Height;
        public PixelFormat PixelFormat;
        public byte[] Bytes;
    }
}
