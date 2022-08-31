using System;
using System.IO;
using Titan.Windows;
using Titan.Windows.Win32;

namespace Titan.FileSystem.Platform;
/// <summary>
/// The Win32 implementation for the File IO functions. This is the preferred API on windows because the File/RandomAccess API requires the managed type SafeFileHandle to read/write files.
/// </summary>
internal unsafe class Win32FileSystemApi : IFileSystemApi
{
    public static FileHandle OpenFile(ReadOnlySpan<char> path, FileAccess access, FileMode mode)
    {
        var desiredAccess = access switch
        {
            FileAccess.Read => GENERIC_RIGHTS.GENERIC_READ,
            FileAccess.Write => GENERIC_RIGHTS.GENERIC_WRITE,
            FileAccess.ReadWrite => GENERIC_RIGHTS.GENERIC_WRITE | GENERIC_RIGHTS.GENERIC_READ,
            _ => GENERIC_RIGHTS.GENERIC_ALL
        };
        fixed (char* pPath = path)
        {
            var handle = Kernel32.CreateFileW(pPath, (uint)desiredAccess, 0, null, (int)CREATION_DISPOSITION.OPEN_EXISTING, (uint)FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL, default);
            if (handle.IsValid())
            {
                return new() { Handle = handle };
            }
        }
        return FileHandle.Invalid;
    }

    public static void CloseFile(ref FileHandle fileHandle)
    {
        Kernel32.CloseHandle(new HANDLE { Value = (nuint)fileHandle.Handle });
    }

    public static int Read(in FileHandle fileHandle, Span<byte> buffer, ulong offset)
    {
        fixed (byte* pBuffer = buffer)
        {
            return Read(fileHandle, pBuffer, (nuint)buffer.Length, offset);
        }
    }

    public static int Read(in FileHandle fileHandle, void* buffer, nuint bufferSize, ulong offset)
    {
        DWORD bytesRead;
        if (Kernel32.ReadFile(fileHandle.Handle, buffer, (uint)bufferSize, &bytesRead, null))
        {
            return (int)bytesRead.Value;
        }
        return -1;
    }

    public static long GetLength(in FileHandle fileHandle)
    {
        LARGE_INTEGER fileSize;
        if (Kernel32.GetFileSizeEx(fileHandle.Handle, &fileSize))
        {
            return (long)fileSize.QuadPart;
        }
        return -1;
    }
}
