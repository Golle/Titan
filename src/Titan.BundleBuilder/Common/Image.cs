using Titan.Windows.Win32.D3D11;

namespace Titan.BundleBuilder.Common
{
    public class Image
    {
        public DXGI_FORMAT Format { get; }
        public uint Stride { get; }
        public uint Width { get; }
        public uint Height { get; }
        public uint ImageSize { get; }
        public byte[] Buffer { get; }
        public Image(byte[] buffer, uint imageSize, DXGI_FORMAT format, uint stride, uint width, uint height)
        {
            Format = format;
            Stride = stride;
            Width = width;
            Height = height;
            Buffer = buffer;
            ImageSize = imageSize;
        }
    }
}
