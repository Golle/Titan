// Comment out this line to disable tracing
#define TRACE_FILE_API

using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Platform.Win32;

namespace Titan.Core.IO.Platform;

internal unsafe class Win32FileApi : IFileApi
{
    public static FileHandle Open(string path, FileAccess access, FileMode mode)
    {
        Trace($"Open file at path {path} with Access: {access} and Mode: {mode}");
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
                return new(handle);
            }
        }
        return FileHandle.Invalid;
    }

    public static int Read(in FileHandle handle, Span<byte> buffer, ulong offset)
    {
        fixed (byte* pBuffer = buffer)
        {
            return Read(handle, pBuffer, (nuint)buffer.Length, offset);
        }
    }


    public static int Read(in FileHandle handle, void* buffer, nuint bufferSize, ulong offset)
    {
        Trace($"Read {bufferSize} bytes from file {handle} at offset {offset}");
        //NOTE(Jens): We need a way to handle big files/reads. Not a problem at the moment. 
        Debug.Assert(bufferSize < int.MaxValue);
        Debug.Assert(offset < uint.MaxValue, $"Offsets greater than {uint.MaxValue} is not supported yet.");
        uint bytesRead;
        OVERLAPPED overlapped = new()
        {
            Offset = (uint)offset
        };
        if (Kernel32.ReadFile(handle.Handle, buffer, (uint)bufferSize, &bytesRead, &overlapped))
        {
            return (int)bytesRead;
        }
        return -1;
    }

    public static int Write(in FileHandle handle, ReadOnlySpan<byte> buffer)
    {
        Trace($"Write {buffer.Length} bytes to file {handle}");
        Debug.Fail("Write has not been implemented yet.");
        return -1;
    }

    public static void Close(ref FileHandle handle)
    {
        Trace($"Closing file: {handle}");
        Kernel32.CloseHandle(new HANDLE { Value = handle.Handle });
    }

    public static long GetLength(in FileHandle handle)
    {
        Trace($"GetLength of file: {handle}");
        LARGE_INTEGER fileSize;
        if (Kernel32.GetFileSizeEx(handle.Handle, &fileSize))
        {
            Trace($"File {handle} length: {fileSize.QuadPart} bytes.");
            return (long)fileSize.QuadPart;
        }
        return -1;
    }


    [Conditional("TRACE_FILE_API")]
    private static void Trace(string message) => Logger.Trace<Win32FileApi>(message);
}
