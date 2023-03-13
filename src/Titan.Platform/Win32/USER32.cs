using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.DBT;

namespace Titan.Platform.Win32;

public static unsafe partial class USER32
{
    public const int GWLP_USERDATA = -21;

    private const string DllName = "User32";

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial ushort RegisterClassExW(
        WNDCLASSEXA* wndClassEx
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnregisterClassW(
        char* LpszClassName,
        HINSTANCE hInstance
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HWND CreateWindowExW(
        WINDOWSTYLES_EX dwExStyle,
        char* lpClassName,
        char* lpWindowName,
        WINDOWSTYLES dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        HWND hWndParent,
        nint hMenu,
        HINSTANCE hInstance,
        void* lpParam
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DestroyWindow(
        HWND hWnd
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial int SetWindowTextW(
        HWND hWnd,
        char* lpString
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AdjustWindowRect(
        RECT* lpRect,
        WINDOWSTYLES dwStyle,
        [MarshalAs(UnmanagedType.Bool)] bool bMenu
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial nint SetWindowLongPtrW(
        HWND hwnd,
        int nIndex,
        nint dwNewLong
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial nint GetWindowLongPtrW(
        HWND hwnd,
        int nIndex
    );


    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial nint GetWindowLongPtrA(
        HWND hwnd,
        int nIndex
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial nint DefWindowProcW(
        HWND hWnd,
        WINDOW_MESSAGE msg,
        nuint wParam,
        nuint lParam
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PeekMessageW(
        MSG* lpMsg,
        HWND hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax,
        uint removeMessage
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TranslateMessage(
        MSG* lpMsg
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial nint DispatchMessageW(
        MSG* lpMsg
    );
    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial void PostQuitMessage(
        int nExitCode
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial int ShowWindow(
        HWND hWnd,
        SHOW_WINDOW_COMMANDS nCmdShow
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(
        POINT* lpPoint
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ScreenToClient(
        HWND hWnd,
        POINT* lpPoint
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetCursorPos(
        int x,
        int y
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HDEVNOTIFY RegisterDeviceNotificationA(
        HANDLE hRecipient,
        void* NotificationFilter,
        DEVICE_NOTIFY_FLAGS Flags
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HDEVNOTIFY RegisterDeviceNotificationW(
        HANDLE hRecipient,
        void* NotificationFilter,
        DEVICE_NOTIFY_FLAGS Flags
    );
}
