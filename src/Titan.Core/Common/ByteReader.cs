using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Titan.Core.Common
{
    public class ByteReader : IDisposable
    {
        private readonly Stream _stream;
        public ByteReader(Stream stream)
        {
            _stream = stream;
        }

        public void Read<T>(out T value) where T : unmanaged
        {
            unsafe
            {
                fixed (void* dataPtr = &value)
                {
                    _stream.Read(new Span<byte>(dataPtr, sizeof(T)));
                }
            }
        }
        public void Read<T>(ref T[] values) where T : unmanaged
        {
            unsafe
            {
                fixed (void* dataPtr = values)
                {
                    Read<T>(dataPtr, (uint) values.Length);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Read<T>(void* output, uint count) where T : unmanaged
        {
            _stream.Read(new Span<byte>(output, (int) (sizeof(T) * count)));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _stream.Dispose();
        }
    }
}
