using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;

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
    public static extern HANDLE CreateEventExA(
        SECURITY_ATTRIBUTES* lpEventAttributes,
        byte* lpName,
        DWORD dwFlags,
        AccessRights dwDesiredAccess);

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern HANDLE CreateEventExW(
        SECURITY_ATTRIBUTES* lpEventAttributes,
        char* lpName,
        DWORD dwFlags,
        AccessRights dwDesiredAccess);

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern DWORD WaitForSingleObject(
        HANDLE hHandle,
        DWORD dwMilliseconds
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(HANDLE handle);

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetDllDirectoryA(
        byte* lpPathName
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetDllDirectoryW(
        char* lpPathName
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern HANDLE CreateFileW(
        char* lpFileName,
        DWORD dwDesiredAccess,
        DWORD dwShareMode,
        SECURITY_ATTRIBUTES* lpSecurityAttributes,
        DWORD dwCreationDisposition,
        DWORD dwFlagsAndAttributes,
        HANDLE hTemplateFile
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadFile(
        HANDLE hFile,
        void* lpBuffer,
        DWORD nNumberOfBytesToRead,
        DWORD* lpNumberOfBytesRead,
        OVERLAPPED* lpOverlapped
    );

    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetFileSizeEx(
        HANDLE hFile,
        LARGE_INTEGER* lpFileSize
    );

    [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern void GetSystemInfo(
        SYSTEM_INFO* lpSystemInfo
    );
}
