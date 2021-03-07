using System;
using System.IO;

namespace Titan.AssetConverter.Common
{
    internal unsafe class ByteStreamReader  : IDisposable
    {
        private readonly Stream _stream;

        public ByteStreamReader(Stream stream)
        {
            _stream = stream;
        }

        public void Read<T>(out T data) where T : unmanaged
        {
            fixed (void* dataPtr = &data)
            {
                _stream.Read(new Span<byte>(dataPtr, sizeof(T)));
            }
        }

        public void Read<T>(ref T[] values) where T : unmanaged
        {
            fixed (void* pData = values)
            {
                _stream.Read(new Span<byte>(pData, (sizeof(T) * values.Length)));
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
