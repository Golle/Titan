using System;
using System.IO;

namespace Titan.FileSystem;

public interface IFileSystemApi
{
    static abstract FileHandle OpenFile(ReadOnlySpan<char> path, FileAccess access, FileMode mode);
    static abstract void CloseFile(ref FileHandle fileHandle);
    static abstract int Read(in FileHandle fileHandle, Span<byte> buffer, ulong offset);
    static abstract unsafe int Read(in FileHandle fileHandle, void* buffer, nuint bufferSize, ulong offset);
    static abstract long GetLength(in FileHandle fileHandle);
}
