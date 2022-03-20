using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

public unsafe class User32
{
    public const int GWLP_USERDATA = -21;

    internal delegate nint WndProcDelegate(HWND hWnd, WindowsMessage msg, nuint wParam, nuint lParam);

    private const string User32Dll = "user32";

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern ushort RegisterClassExA(
        [In] in WNDCLASSEXA wndClassEx
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern int ShowWindow(
        [In] HWND hWnd,
        [In] ShowWindowCommand nCmdShow
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HWND CreateWindowExA(
        [In] WindowStylesEx dwExStyle,
        [In] string lpClassName,
        [In] string lpWindowName,
        [In] WindowStyles dwStyle,
        [In] int x,
        [In] int y,
        [In] int nWidth,
        [In] int nHeight,
        [In] HWND hWndParent,
        [In] nint hMenu,
        [In] nint hInstance,
        [In] void* lpParam
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint DefWindowProcA(
        [In] HWND hWnd,
        [In] WindowsMessage msg,
        [In] nuint wParam,
        [In] nuint lParam
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PeekMessageA(
        [Out] out MSG lpMsg,
        [In] HWND hWnd,
        [In] uint wMsgFilterMin,
        [In] uint wMsgFilterMax,
        [In] uint removeMessage
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TranslateMessage(
        [In] in MSG lpMsg
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint DispatchMessage(
        [In] in MSG lpMsg
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern void PostQuitMessage(
        [In] int nExitCode
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(
        [In] HWND hWnd
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterClassA(
        [In, MarshalAs(UnmanagedType.LPStr)] string LpszClassName,
        [In] nint hInstance
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowTextA(
        [In] HWND hWnd,
        [In] string lpString
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(
        [Out] POINT* lpPoint
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCursorPos(
        [In] int x,
        [In] int y
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HCURSOR LoadCursorW(
        [In] nint hInstance,
        [In] char* lpCursorName
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern int ShowCursor(
        [In, MarshalAs(UnmanagedType.Bool)] bool show
    );


    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern int SetCursor(
        [In] HCURSOR cursor
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ScreenToClient(
        [In] HWND hWnd,
        [In] POINT* lpPoint
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AdjustWindowRect(
        [In] ref RECT lpRect,
        [In] WindowStyles dwStyle,
        [In, MarshalAs(UnmanagedType.Bool)] bool bMenu
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint SetWindowLongPtrA(
        [In] HWND hwnd,
        [In] int nIndex,
        [In] nint dwNewLong
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern nint GetWindowLongPtrA(
        [In] HWND hwnd,
        [In] int nIndex
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern int GetSystemMetrics(SystemMetricsCode index);

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SystemParametersInfoA(
        SystemParametersInfo uiAction,
        uint uiParam,
        void* pvParam,
        SystemParametersInfoFlags fWinIni
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterRawInputDevices(
        RAWINPUTDEVICE* pRawInputDevices,
        uint uiNumDevices,
        uint cbSize
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern uint GetRawInputData(
        HRAWINPUT hRawInput,
        RIDCOMMAND uiCommand,
        void* pData,
        uint* pcbSize,
        uint cbSizeHeader
    );


    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern uint GetRawInputDeviceInfoW(
        HANDLE hDevice,
        RIDICOMMAND uiCommand,
        void* pData,
        uint* pcbSize
    );
    
    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern uint GetRawInputDeviceInfoA(
        HANDLE hDevice,
        RIDICOMMAND uiCommand,
        void* pData,
        uint* pcbSize
    );

    [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern uint GetRawInputBuffer(
        RAWINPUT* pData,
        uint* pcbSize,
        uint cbSizeHeader
    );
}
