using System;
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


    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern HANDLE CreateEventW(
        SECURITY_ATTRIBUTES* lpEventAttributes,
        int bManualReset,
        int bInitialState,
        char* lpName
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern HANDLE CreateEventA(
        SECURITY_ATTRIBUTES* lpEventAttributes,
        int bManualReset,
        int bInitialState,
        byte* lpName
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern DWORD WaitForSingleObject(
        HANDLE hHandle,
        DWORD dwMilliseconds
    );
}
