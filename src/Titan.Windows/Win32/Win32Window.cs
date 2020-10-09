using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Titan.Windows.Win32.Native;

using static Titan.Windows.Win32.Native.User32;

namespace Titan.Windows.Win32
{
    internal class Win32Window : IWindow
    {
        public HWND Handle { get; }
        public int Height { get; }
        public int Width { get; }
        public unsafe Win32Window(int width, int height, string title)
        {
            Width = width;
            Height = height;


            var className = $"{nameof(Win32Window)}_class_" + Guid.NewGuid().ToString().Substring(0, 4);
            // Create the Window Class EX
            var wndClassExA = new WndClassExA
            {
                CbClsExtra = 0,
                CbSize = (uint)Marshal.SizeOf<WndClassExA>(),
                HCursor = 0,
                HIcon = 0,
                //LpFnWndProc = (delegate*<nint, WindowsMessage, nuint, nuint, nint>)wndProcPointer.ToPointer(),
                LpFnWndProc = &StaticWindowProc,
                CbWndExtra = 0,
                HIconSm = 0,
                HInstance = Marshal.GetHINSTANCE(GetType().Module),
                HbrBackground = 0,
                LpszClassName = className,
                Style = 0
            };
            if (RegisterClassExA(wndClassExA) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "RegisterClassExA failed");
            }

            // Adjust the window size to take into account for the menu etc
            const WindowStyles wsStyle = WindowStyles.OverlappedWindow | WindowStyles.Visible;
            Rect windowRect = default;
            windowRect.Left = 100;
            windowRect.Right = width + windowRect.Left;
            windowRect.Top = 100;
            windowRect.Bottom = height + windowRect.Top;
            AdjustWindowRect(ref windowRect, wsStyle, false);


            // Create the Window
            Handle = CreateWindowExA(
                0,
                className,
                title,
                wsStyle,
                -1,
                -1,
                windowRect.Right - windowRect.Left,
                windowRect.Bottom - windowRect.Top,
                0,
                0,
                wndClassExA.HInstance,
                ((IntPtr)GCHandle.Alloc(this)).ToPointer()
            );

            if (Handle == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateWindowExA failed");
            }

            // Show the window
            Show();
        }

        public void SetTitle(string title) => SetWindowTextA(Handle, title);
        public void Hide() => ShowWindow(Handle, ShowWindowCommand.Hide);
        public void Show() => ShowWindow(Handle, ShowWindowCommand.Show);


        private static unsafe nint StaticWindowProc(HWND hWnd, WindowsMessage message, nuint wParam, nuint lParam)
        {
            var handle = GetWindowLongPtrA(hWnd, GWLP_USERDATA);
            var window = handle == 0 ? null : (Win32Window)GCHandle.FromIntPtr(handle).Target;
            if (window != null)
            {
                return window.WindowProcedure(hWnd, message, wParam, lParam);
            }

            switch (message)
            {
                case WindowsMessage.Create:
                {
                    var pCreateStruct = (CREATESTRUCTA*)lParam;
                    _ = SetWindowLongPtrA(hWnd, GWLP_USERDATA, (nint)pCreateStruct->lpCreateParams);
                    return 0;
                }
            }
            return DefWindowProcA(hWnd, message, wParam, lParam);
        }

        private nint WindowProcedure(HWND hWnd, WindowsMessage message, nuint wParam, nuint lParam)
        {
            switch (message)
            {
                case WindowsMessage.Close:
                    PostQuitMessage(0);
                    return 0;
            }
            return DefWindowProcA(hWnd, message, wParam, lParam);
        }

        public bool Update()
        {
            while (PeekMessageA(out var msg, 0, 0, 0, 1)) // pass IntPtr.Zero as HWND to detect mouse movement outside of the window
            {
                if (msg.Message == WindowsMessage.Quit)
                {
                    var pHandle = SetWindowLongPtrA(Handle, GWLP_USERDATA, 0);
                    if (pHandle != 0)
                    {
                        var handle = GCHandle.FromIntPtr(pHandle);
                        if (handle.IsAllocated)
                        {
                            handle.Free();
                        }
                    }
                    return false;
                }
                TranslateMessage(msg);
                DispatchMessage(msg);
            }
            return true;
        }

        public void Dispose()
        {
            
        }
    }
}
