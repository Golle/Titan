using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Titan.BundleBuilder.Common
{
    internal unsafe class ByteStreamWriter : IDisposable
    {
        private readonly Stream _stream;
        public ByteStreamWriter(Stream stream)
        {
            _stream = stream;
        }

        public void Write<T>(in T data) where T : unmanaged
        {
            fixed (T* pData = &data)
            {
                _stream.Write(new ReadOnlySpan<byte>(pData, sizeof(T)));
            }
        }

        public void Write<T>(ReadOnlyMemory<T> memory) where T : unmanaged
        {
            if (!MemoryMarshal.TryGetArray(memory, out var data))
            {
                throw new NotSupportedException("Failed to get the underlying array");
            }
            fixed (T* pData = data.Array)
            {
                _stream.Write(new ReadOnlySpan<byte>(pData, sizeof(T) * memory.Length));
            }
        }


        public Task FlushAsync() => _stream.FlushAsync();
        public void Flush() => _stream.Flush();

        public void Dispose()
        {
            _stream.Flush();
            _stream?.Dispose();
        }

        
    }
}
