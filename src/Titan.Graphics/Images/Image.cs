using System;
using System.Runtime.InteropServices;
using Titan.Windows.DXGI;

namespace Titan.Graphics.Images
{
    public class Image : IDisposable
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
        public unsafe void Dispose()
        {
            if (_buffer != null)
            {
                NativeMemory.Free(_buffer);
                _buffer = null;
                _imageSize = 0;
            }
        }
    }
}
