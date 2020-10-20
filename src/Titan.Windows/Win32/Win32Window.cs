using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows.Win32.Native;

using static Titan.Windows.Win32.Native.User32;
using static Titan.Windows.Win32.Native.WindowsMessage;

namespace Titan.Windows.Win32
{
    internal class Win32Window : IWindow
    {
        private struct UserData
        {
            public IntPtr Window;
            public IntPtr EventHandler;
        }

        private readonly IWindowEventHandler _windowEventHandler;
        public HWND Handle { get; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public POINT Center { get; private set; }
        public bool Windowed => true;

        private readonly UserData _userData;
        private GCHandle _userDataHandle;

        public unsafe Win32Window(int width, int height, string title, IWindowEventHandler windowEventHandler)
        {
            _windowEventHandler = windowEventHandler;
            _userData.EventHandler = (IntPtr) GCHandle.Alloc(_windowEventHandler);
            _userData.Window = (IntPtr) GCHandle.Alloc(this);
            _userDataHandle = GCHandle.Alloc(_userData, GCHandleType.Pinned);

            Width = width;
            Height = height;

            var className = $"{nameof(Win32Window)}_class_" + Guid.NewGuid().ToString().Substring(0, 4);
            // Create the Window Class EX
            var wndClassExA = new WNDCLASSEXA
            {
                CbClsExtra = 0,
                CbSize = (uint)Marshal.SizeOf<WNDCLASSEXA>(),
                HCursor = 0,
                HIcon = 0,
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
            RECT windowRect = default;
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
                Unsafe.AsPointer(ref _userData)
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
            GetUserData(hWnd, out var eventHandler, out var window);

            switch (message)
            {
                case WM_KILLFOCUS:
                    eventHandler?.OnLostFocus();
                    break;
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    {
                        var repeat = (lParam & 0x40000000) > 0;
                        var code = (KeyCode)wParam;
                        eventHandler?.OnKeyDown(code, repeat);
                    }
                    break;
                case WM_KEYUP:
                case WM_SYSKEYUP:
                    eventHandler?.OnKeyUp((KeyCode)wParam);
                    break;
                case WM_CHAR:
                    eventHandler?.OnCharTyped((char)wParam);
                    break;
                case WM_SIZE:
                {
                    var width = (int)(lParam & 0xffff);
                    var height = (int)((lParam >> 16) & 0xffff);
                    if (window != null)
                    {
                        window.Height = height;
                        window.Width = width;
                        window.Center = new POINT {X = window.Width / 2, Y = window.Height};
                    }
                    eventHandler?.OnWindowResize(width, height);
                }
                    
                    break;
                case WM_EXITSIZEMOVE:
                    break;

                case WM_LBUTTONDOWN:
                    break;
                case WM_LBUTTONUP:
                    break;
                case WM_RBUTTONDOWN:
                    break;
                case WM_RBUTTONUP:
                    break;

                
                case WM_MOUSELEAVE:
                    break;
                case WM_MOUSEWHEEL:

                    break;
                case WM_CREATE:
                    {
                        var userData = (UserData*)((CREATESTRUCTA*)lParam)->lpCreateParams;
                        _ = SetWindowLongPtrA(hWnd, GWLP_USERDATA, (nint) userData);
                        if (userData != null)
                        {
                            ((IWindowEventHandler)GCHandle.FromIntPtr(userData->EventHandler).Target)?.OnCreate();
                        }
                        return 0;
                    }
                
                case WM_CLOSE:
                    eventHandler?.OnClose();
                    PostQuitMessage(0);
                    return 0;
            }
            return DefWindowProcA(hWnd, message, wParam, lParam);
        }

        private static unsafe void GetUserData(HWND hWnd, out IWindowEventHandler eventHandler, out Win32Window window)
        {
            var userData = (UserData*)GetWindowLongPtrA(hWnd, GWLP_USERDATA);
            if (userData != null)
            {
                eventHandler = (IWindowEventHandler) GCHandle.FromIntPtr(userData->EventHandler).Target;
                window = (Win32Window) GCHandle.FromIntPtr(userData->Window).Target;
            }
            else
            {
                eventHandler = null;
                window = null;
            }
        }

        public bool Update()
        {
            while (PeekMessageA(out var msg, 0, 0, 0, 1)) // pass IntPtr.Zero as HWND to detect mouse movement outside of the window
            {
                if (msg.Message == WM_QUIT)
                {
                    SetWindowLongPtrA(Handle, GWLP_USERDATA, 0);
                    return false;
                }
                TranslateMessage(msg);
                DispatchMessage(msg);
            }
            return true;
        }

        public void Dispose()
        {
            GCHandle.FromIntPtr(_userData.EventHandler).Free();
            GCHandle.FromIntPtr(_userData.Window).Free();
            _userDataHandle.Free();

            DestroyWindow(Handle);
        }
    }
}
