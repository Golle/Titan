using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.WIC;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.Textures
{
    internal class ImageDecoder : IImageDecoder
    {
        public uint Width { get; }
        public uint Height { get; }
        public uint BitsPerPixel { get; }
        public uint ImageSize => Stride * Height;
        public uint Stride => (Width * BitsPerPixel + 7) / 8;
        public Guid PixelFormat { get; }
        private ComPtr<IWICBitmapFrameDecode> _frameDecode;
        public ImageDecoder(ComPtr<IWICBitmapFrameDecode> frame, uint width, uint height, in Guid pixelFormat, uint bitsPerPixel)
        {
            _frameDecode = new ComPtr<IWICBitmapFrameDecode>(frame);
            BitsPerPixel = bitsPerPixel;
            PixelFormat = pixelFormat;
            Width = width;
            Height = height;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyPixels(IntPtr buffer, uint size)
        {
            unsafe
            {
                CopyPixels((byte*) buffer.ToPointer(), size);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void CopyPixels(byte* buffer, uint size) => CheckAndThrow(_frameDecode.Get()->CopyPixels(null, Stride, size, buffer), "CopyPixels");

        public void Dispose() => _frameDecode.Dispose();
    }
}
