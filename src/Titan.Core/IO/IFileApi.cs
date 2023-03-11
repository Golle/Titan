namespace Titan.Core.IO;

internal interface IFileApi
{
    static abstract FileHandle Open(string path, FileAccess access, FileMode mode);
    static abstract int Read(in FileHandle handle, Span<byte> buffer, ulong offset);
    static abstract unsafe int Read(in FileHandle handle, void* buffer, nuint bufferSize, ulong offset);
    static abstract int Write(in FileHandle handle, ReadOnlySpan<byte> buffer);
    static abstract void Close(ref FileHandle handle);
    static abstract long GetLength(in FileHandle handle);
}
