using System;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32
{
    internal class User32
    {
        internal delegate nint WndProcDelegate(nint hWnd, WindowsMessage msg, nuint wParam, nuint lParam);

        private const string User32Dll = "user32";

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern ushort RegisterClassExA(
            [In] in WndClassExA wndClassEx
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ShowWindow(
            [In] nint hWnd,
            [In] ShowWindow nCmdShow
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern nint CreateWindowExA(
            [In] WindowStylesEx dwExStyle,
            [In] string lpClassName,
            [In] string lpWindowName,
            [In] WindowStyles dwStyle,
            [In] int x,
            [In] int y,
            [In] int nWidth,
            [In] int nHeight,
            [In] nint hWndParent,
            [In] nint hMenu,
            [In] nint hInstance,
            [In] nint lpParam
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern nint DefWindowProcA(
            [In] nint hWnd,
            [In] WindowsMessage msg,
            [In] nuint wParam,
            [In] nuint lParam
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessageA(
            [Out] out Msg lpMsg,
            [In] nint hWnd,
            [In] uint wMsgFilterMin,
            [In] uint wMsgFilterMax,
            [In] uint removeMessage
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage(
            [In] in Msg lpMsg
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern nint DispatchMessage(
            [In] in Msg lpMsg
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern void PostQuitMessage(
            [In] int nExitCode
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(
            [In] nint hWnd
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowTextA(
            [In] nint hWnd,
            [In] string lpString
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(
            [Out] out Point lpPoint
        );
        
        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(
            [In] int x,
            [In] int y
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ShowCursor(
            [In, MarshalAs(UnmanagedType.Bool)] bool show
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ScreenToClient(
            [In] nint hWnd,
            [In, Out] ref Point lpPoint
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustWindowRect(
            [In] ref Rect lpRect,
            [In] WindowStyles dwStyle,
            [In, MarshalAs(UnmanagedType.Bool)] bool bMenu
        );
    }
}
