using System;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Titan.Core.IO.NewFileSystem;

public struct ManagedFileSystemApi : IFileSystemApi
{
    public static FileHandle OpenFile(ReadOnlySpan<char> path, FileAccess access, FileMode mode)
    {
        var handle = File.OpenHandle(new string(path), mode, access, FileShare.None, FileOptions.RandomAccess);
        if (handle.IsInvalid)
        {
            // Log?
            return FileHandle.Invalid;
        }
        return handle.DangerousGetHandle();
    }

    public static void CloseFile(ref FileHandle fileHandle)
    {
        if (fileHandle.IsValid)
        {
            //NOTE(Jens): This will allocate memory. But we'll most likely only open very few files during runtime
            new SafeFileHandle(fileHandle.Handle, true)
                .Close();
        }
        fileHandle = FileHandle.Invalid;

    }

    public static int Read(in FileHandle fileHandle, Span<byte> buffer, ulong offset) 
        => RandomAccess.Read(new SafeFileHandle(fileHandle.Handle, true), buffer, (long)offset); //NOTE(Jens): this will generate garbage on each read. not sure if it's a problem.

    public static unsafe int Read(in FileHandle fileHandle, void* buffer, nuint bufferSize, ulong offset) 
        => Read(fileHandle, new Span<byte>(buffer, (int)bufferSize), offset); // Will overflow for big sizes

    public static long GetLength(in FileHandle fileHandle) 
        => RandomAccess.GetLength(new SafeFileHandle(fileHandle, true));
}
