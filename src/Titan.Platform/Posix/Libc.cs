using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Posix;

public static unsafe partial class Libc
{
    private const string DllName = "libc";

    //https://sites.uclouvain.be/SystInfo/usr/include/bits/mman.h.html
    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int getpagesize();

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial void* mmap(void* addr, nuint length, PageProtection prot, PageFlags flags, int fd, ulong offset);

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int munmap(void* addr, nuint length);
    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int mprotect(void* addr, nuint len, PageProtection prot);

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int open(byte* pathname, int flags, mode_t mode);

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int close(int fd);

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial nuint read(int fd, void* buf, nuint count);

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int fstat(int fd, PosixStat* statbuf);

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial long lseek(int fd, long offset, int whence);
}

