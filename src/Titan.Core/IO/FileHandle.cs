using System.Runtime.CompilerServices;
using Microsoft.Win32.SafeHandles;

namespace Titan.Core.IO
{
    public readonly struct FileHandle : IDisposable
    {
        private readonly SafeFileHandle _fileHandle;

        public FileHandle(SafeFileHandle fileHandle) => _fileHandle = fileHandle;

        public long Length => RandomAccess.GetLength(_fileHandle);
        public int Read(in Span<byte> buffer) => RandomAccess.Read(_fileHandle, buffer, 0);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadAllBytes()
        {
            var buffer = new byte[Length];
            RandomAccess.Read(_fileHandle, buffer, 0);
            return buffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => _fileHandle.Dispose();

    }
}
