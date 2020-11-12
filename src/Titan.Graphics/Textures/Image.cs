using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Textures
{
    internal class Image : IImage
    {
        public DXGI_FORMAT Format { get; }
        public uint Stride { get; }
        public uint Width { get; }
        public uint Height { get; }
        
        private uint _imageSize;
        private unsafe byte* _buffer;
        public unsafe Image(byte* buffer, uint imageSize, DXGI_FORMAT format, uint stride, uint width, uint height)
        {
            Format = format;
            Stride = stride;
            Width = width;
            Height = height;
            _buffer = buffer;
            _imageSize = imageSize;
        }
        public unsafe byte* GetBuffer() => _buffer;
        public uint GetBufferSize() => _imageSize;

        ~Image() => Dispose();
        public void Dispose()
        {
            unsafe
            {
                if (_buffer != null)
                {
                    Marshal.FreeHGlobal((nint)_buffer);
                    _buffer = null;
                    _imageSize = 0;
                }
            }
        }
    }
}
