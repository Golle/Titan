using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.FileSystem;

public unsafe struct FileSystemApi : IApi
{
    private delegate*<ReadOnlySpan<char>, FileAccess, FileMode, FileHandle> _openFile;
    private delegate*<ref FileHandle, void> _closeFile;
    private delegate*<in FileHandle, Span<byte>, ulong, int> _read;
    private delegate*<in FileHandle, void*, nuint, ulong, int> _readPtr;
    private delegate*<in FileHandle, long> _length;
    public static FileSystemApi Create<T>() where T : IFileSystemApi =>
        new()
        {
            _openFile = &T.OpenFile,
            _closeFile = &T.CloseFile,
            _read = &T.Read,
            _readPtr = &T.Read,
            _length = &T.GetLength
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FileHandle Open(ReadOnlySpan<char> path, FileAccess access = FileAccess.Read, FileMode mode = FileMode.Open) => _openFile(path, access, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Close(ref FileHandle handle) => _closeFile(ref handle);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int Read(in FileHandle handle, Span<byte> buffer, ulong offset = 0ul) => _read(handle, buffer, offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int Read(in FileHandle handle, void* buffer, nuint bufferSize, ulong offset = 0ul) => _readPtr(handle, buffer, bufferSize, offset);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly long GetLength(in FileHandle handle) => _length(handle);
}
