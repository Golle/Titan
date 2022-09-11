using System.Runtime.InteropServices;

namespace Titan.Platform.Posix;

public unsafe class Libc
{
    //https://sites.uclouvain.be/SystInfo/usr/include/bits/mman.h.html
    [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getpagesize();

    [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* mmap(void* addr, nuint length, PageProtection prot, PageFlags flags, int fd, ulong offset);

    [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
    public static extern int munmap(void* addr, nuint length);
    [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
    public static extern int mprotect(void* addr, nuint len, PageProtection prot);
}
