using System.Diagnostics;
using System.Text;
using Titan.Platform.Posix;
using static Titan.Platform.Posix.Libc;

namespace Titan.Core.IO.Platform;

public unsafe class PosixFileSystem : IFileApi
{
    private const int O_RDONLY = 0x00;
    private const int O_WRONLY = 0x01;
    private const int O_RDWR = 0x02;

    private const int SEEK_SET = 0;
    private const int SEEK_CUR = 1;
    private const int SEEK_END = 2;

    public static FileHandle Open(string path, FileAccess access, FileMode mode)
    {
        var length = path.Length * 2 + 1;
        var utf8Path = stackalloc byte[length];
        length = Encoding.UTF8.GetBytes(path, new Span<byte>(utf8Path, length));
        utf8Path[length] = 0;

        Debug.Assert(access == FileAccess.Read, "Only Read is currently supported");
        Debug.Assert(mode == FileMode.Open, "Only Open is currently supported");

        var file = open(utf8Path, O_RDONLY, 0);
        if (file == 0)
        {
            //Logger.Trace<PosixFileSystem>("Failed to open file");
            return FileHandle.Invalid;
        }
        //Logger.Trace<PosixFileSystem>("File opened");
        return new((nuint)file);
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
        Debug.Assert(handle.IsValid());
        var fd = (int)handle.Handle;
        var res = lseek(fd, (long)offset, SEEK_SET);
        Debug.Assert(res != -1);
        return (int)read(fd, buffer, bufferSize);
    }

    public static int Write(in FileHandle handle, ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public static void Close(ref FileHandle handle)
    {
        var fd = (int)handle.Handle;
        var result = close(fd);
        Debug.Assert(result == 0);
        handle = default;
    }

    public static long GetLength(in FileHandle handle)
    {
        var fd = (int)handle.Handle;
        PosixStat stat;
        var result = fstat(fd, &stat);
        if (result == 0)
        {
            return (long)stat.st_size;
        }
        return -1;
    }
}
