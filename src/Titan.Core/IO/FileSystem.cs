using System.Runtime.CompilerServices;

namespace Titan.Core.IO;

//NOTE(Jens): This separation is to make it easier to transition to unmanaged memory if we want that. This class is unnecessary in it's current state, it's only here to make it possible for platform specific code to be in static functions.
internal class FileSystem<T> : IFileSystem where T : IFileApi
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FileHandle Open(string path, FileAccess access, FileMode mode) => T.Open(path, access, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(in FileHandle handle, Span<byte> buffer, ulong offset = 0) => T.Read(handle, buffer, offset);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int Read(in FileHandle handle, void* buffer, nuint bufferSize, ulong offset = 0) => T.Read(handle, buffer, bufferSize, offset);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write(in FileHandle handle, ReadOnlySpan<byte> buffer) => T.Write(handle, buffer);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Close(ref FileHandle handle) => T.Close(ref handle);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetLength(in FileHandle handle) => T.GetLength(handle);
}
