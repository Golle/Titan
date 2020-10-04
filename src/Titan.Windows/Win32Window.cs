using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Titan.Windows.Win32;

namespace Titan.Windows
{
    internal class Win32Window : IWindow
    {
        public nint NativeHandle { get; }
        public int Height { get; }
        public int Width { get; }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly User32.WndProcDelegate _wndProcDelegate; // Prevent the delegate from being garbage collected
        private GCHandle _wndProdPinnedMemory;
        private readonly string _className = $"{nameof(Win32Window)}_class_" + Guid.NewGuid().ToString().Substring(0, 4);
        
        public unsafe Win32Window(int width, int height, string title)
        {
            Width = width;
            Height = height;

            // Set up the WndProc callback
            _wndProcDelegate = WindowProcedure;
            var wndProcPointer = Marshal.GetFunctionPointerForDelegate(_wndProcDelegate);
            _wndProdPinnedMemory = GCHandle.Alloc(wndProcPointer, GCHandleType.Pinned);

            // Create the Window Class EX
            var wndClassExA = new WndClassExA
            {
                CbClsExtra = 0,
                CbSize = (uint)Marshal.SizeOf<WndClassExA>(),
                HCursor = 0,
                HIcon = 0,
                LpFnWndProc = (delegate*<nint, WindowsMessage, nuint, nuint, nint>)wndProcPointer.ToPointer(),
                CbWndExtra = 0,
                HIconSm = 0,
                HInstance = Marshal.GetHINSTANCE(GetType().Module),
                HbrBackground = 0,
                LpszClassName = _className,
                Style = 0
            };
            if (User32.RegisterClassExA(wndClassExA) == 0)
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
            User32.AdjustWindowRect(ref windowRect, wsStyle, false);


            // Create the Window
            NativeHandle = User32.CreateWindowExA(
                0,
                _className,
                title,
                wsStyle,
                -1,
                -1,
                windowRect.Right - windowRect.Left,
                windowRect.Bottom - windowRect.Top,
                0,
                0,
                wndClassExA.HInstance,
                0
            );

            if (NativeHandle == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateWindowExA failed");
            }

            // Show the window
            Show();
        }

        public void SetTitle(string title) => User32.SetWindowTextA(NativeHandle, title);
        public void Hide() => User32.ShowWindow(NativeHandle, ShowWindow.Hide);
        public void Show() => User32.ShowWindow(NativeHandle, ShowWindow.Show);

        private nint WindowProcedure(nint hWnd, WindowsMessage message, nuint wParam, nuint lParam)
        {
            switch (message)
            {
                case WindowsMessage.Close:
                    User32.PostQuitMessage(0);
                    return 0;
            }
            return User32.DefWindowProcA(hWnd, message, wParam, lParam);
        }

        public bool Update()
        {
            while (User32.PeekMessageA(out var msg, 0, 0, 0, 1)) // pass IntPtr.Zero as HWND to detect mouse movement outside of the window
            {
                if (msg.Message == WindowsMessage.Quit)
                {
                    return false;
                }
                User32.TranslateMessage(msg);
                User32.DispatchMessage(msg);
            }
            return true;
        }

        public void Dispose()
        {
            if (_wndProdPinnedMemory.IsAllocated)
            {
                _wndProdPinnedMemory.Free();
            }
        }
    }
}
