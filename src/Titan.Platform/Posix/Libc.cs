using System.Runtime.InteropServices;

namespace Titan.Platform.Posix;

public unsafe class Libc
{
    private const string DllName = "libc";

    //https://sites.uclouvain.be/SystInfo/usr/include/bits/mman.h.html
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getpagesize();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void* mmap(void* addr, nuint length, PageProtection prot, PageFlags flags, int fd, ulong offset);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int munmap(void* addr, nuint length);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mprotect(void* addr, nuint len, PageProtection prot);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int open(byte* pathname, int flags, mode_t mode);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int close(int fd);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint read(int fd, void* buf, nuint count);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int fstat(int fd, PosixStat* statbuf);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern long lseek(int fd, long offset, int whence);
}

