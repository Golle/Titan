using System;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32
{
    internal class User32
    {
        internal delegate IntPtr WndProcDelegate(IntPtr hWnd, WindowsMessage msg, UIntPtr wParam, UIntPtr lParam);

        private const string User32Dll = "user32";

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern ushort RegisterClassExA(
            [In] in WndClassExA wndClassEx
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ShowWindow(
            IntPtr hWnd,
            ShowWindow nCmdShow
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr CreateWindowExA(
            WindowStylesEx dwExStyle,
            string lpClassName,
            string lpWindowName,
            WindowStyles dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr DefWindowProcA(
            [In] IntPtr hWnd,
            [In] WindowsMessage msg,
            [In] UIntPtr wParam,
            [In] UIntPtr lParam
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessageA(
            [Out] out Msg lpMsg,
            [In] IntPtr hWnd,
            [In] uint wMsgFilterMin,
            [In] uint wMsgFilterMax,
            [In] uint removeMessage
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage(
            [In] ref Msg lpMsg
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr DispatchMessage(
            [In] ref Msg lpMsg
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern void PostQuitMessage(
            [In] int nExitCode
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(
            [In] IntPtr hWnd
        );

        [DllImport(User32Dll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowTextA(
            [In] IntPtr hWnd,
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
            [In] IntPtr hWnd,
            ref Point lpPoint
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
