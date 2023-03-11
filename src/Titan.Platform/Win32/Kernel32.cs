using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Titan.Platform.Win32;

public unsafe class Kernel32
{
    private const string DllName = "kernel32";

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern HMODULE GetModuleHandleW(
        char* lpModuleName
    );

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
    public static extern void GetSystemInfo(
        SYSTEM_INFO* lpSystemInfo
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern uint WaitForSingleObject(
        HANDLE hHandle,
        uint dwMilliseconds
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
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(HANDLE handle);

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern HANDLE CreateFileW(
        char* lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        SECURITY_ATTRIBUTES* lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        HANDLE hTemplateFile
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadFile(
        HANDLE hFile,
        void* lpBuffer,
        uint nNumberOfBytesToRead,
        uint* lpNumberOfBytesRead,
        OVERLAPPED* lpOverlapped
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetFileSizeEx(
        HANDLE hFile,
        LARGE_INTEGER* lpFileSize
    );


    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern HANDLE CreateThread(
        SECURITY_ATTRIBUTES* lpThreadAttributes,
        nuint dwStackSize,
        delegate* unmanaged<void*, int> lpStartAddress,
        void* lpParameter,
        uint dwCreationFlags,
        uint* lpThreadId
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern void Sleep(uint milliseconds);
    
    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern uint GetCurrentThreadId();

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern uint ResumeThread(
        HANDLE hThread
    );


    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetDllDirectoryW(
        char* lpPathName
    );
}
