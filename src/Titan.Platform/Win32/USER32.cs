using System.Runtime.InteropServices;
using Titan.Platform.Win32.DBT;

namespace Titan.Platform.Win32;

public static unsafe class USER32
{
    public const int GWLP_USERDATA = -21;

    private const string DllName = "user32";

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern ushort RegisterClassExW(
        WNDCLASSEXA* wndClassEx
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterClassW(
        char* LpszClassName,
        HINSTANCE hInstance
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HWND CreateWindowExW(
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

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(
        HWND hWnd
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern int SetWindowTextW(
        HWND hWnd,
        char* lpString
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AdjustWindowRect(
        RECT* lpRect,
        WINDOWSTYLES dwStyle,
        [MarshalAs(UnmanagedType.Bool)] bool bMenu
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint SetWindowLongPtrW(
        HWND hwnd,
        int nIndex,
        nint dwNewLong
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint GetWindowLongPtrW(
        HWND hwnd,
        int nIndex
    );


    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint GetWindowLongPtrA(
        HWND hwnd,
        int nIndex
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint DefWindowProcW(
        HWND hWnd,
        WINDOW_MESSAGE msg,
        nuint wParam,
        nuint lParam
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PeekMessageW(
        MSG* lpMsg,
        HWND hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax,
        uint removeMessage
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TranslateMessage(
        MSG* lpMsg
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint DispatchMessage(
        MSG* lpMsg
    );
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern void PostQuitMessage(
        int nExitCode
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern int ShowWindow(
        HWND hWnd,
        SHOW_WINDOW_COMMANDS nCmdShow
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(
        POINT* lpPoint
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ScreenToClient(
        HWND hWnd,
        POINT* lpPoint
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCursorPos(
        int x,
        int y
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HDEVNOTIFY RegisterDeviceNotificationA(
        HANDLE hRecipient,
        void* NotificationFilter,
        DEVICE_NOTIFY_FLAGS Flags
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HDEVNOTIFY RegisterDeviceNotificationW(
        HANDLE hRecipient,
        void* NotificationFilter,
        DEVICE_NOTIFY_FLAGS Flags
    );
}
