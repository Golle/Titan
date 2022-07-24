using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

public static unsafe class Kernel32
{
    private const string DllName = "kernel32";

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern void* VirtualAlloc(
        void* lpAddress,
        nuint dwSize,
        AllocationType flAllocationType,
        AllocationProtect flProtect
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool VirtualFree(
        void* lpAddress,
        nuint dwSize,
        AllocationType dwFreeType
    );
}
