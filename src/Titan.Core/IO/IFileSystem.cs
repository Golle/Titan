namespace Titan.Core.IO;

public interface IFileSystem
{
    //NOTE(Jens): Implement our own file mode/access enums
    FileHandle Open(string path, FileAccess access, FileMode mode);
    int Read(in FileHandle handle, Span<byte> buffer, ulong offset = 0ul);
    unsafe int Read(in FileHandle handle, void* buffer, nuint bufferSize, ulong offset = 0ul);
    int Write(in FileHandle handle, ReadOnlySpan<byte> buffer);
    void Close(ref FileHandle handle);
    long GetLength(in FileHandle handle);
}
