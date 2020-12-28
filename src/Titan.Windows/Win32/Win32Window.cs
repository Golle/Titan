using System;
using System.ComponentModel;
using System.Diagnostics;
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
        public HWND Handle { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public POINT Center { get; private set; }
        public bool Windowed => true;

        private unsafe UserData * _userData;
        private POINT _mousePosition;

        public Win32Window(IWindowEventHandler windowEventHandler)
        {
            _windowEventHandler = windowEventHandler;
        }
        public unsafe void Initialize(int width, int height, string title)
        {
            if (_userData != null)
            {
                throw new InvalidOperationException("Window has already been initialized");
            }
            _userData = (UserData*)Marshal.AllocHGlobal(sizeof(UserData));
            _userData->EventHandler = (IntPtr) GCHandle.Alloc(_windowEventHandler);
            _userData->Window = (IntPtr) GCHandle.Alloc(this);

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
                _userData
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

            if (eventHandler == null || window == null)
            {
                goto EndHandler;
            }

            switch (message)
            {
                case WM_KILLFOCUS:
                    eventHandler.OnLostFocus();
                    break;
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    eventHandler.OnKeyDown(wParam,lParam);
                    break;
                case WM_KEYUP:
                case WM_SYSKEYUP:
                    eventHandler.OnKeyUp(wParam);
                    break;
                case WM_CHAR:
                    eventHandler.OnCharTyped(wParam);
                    break;
                case WM_SIZE:
                {
                    var width = (int) (lParam & 0xffff);
                    var height = (int) ((lParam >> 16) & 0xffff);
                    window.Height = height;
                    window.Width = width;
                    window.Center = new POINT {X = window.Width / 2, Y = window.Height / 2};
                }
                    break;
                case WM_EXITSIZEMOVE:
                    eventHandler.OnWindowResize(window.Width, window.Height);
                    break;
                case WM_LBUTTONDOWN:
                    eventHandler.OnLeftMouseButtonDown();
                    break;
                case WM_LBUTTONUP:
                    eventHandler.OnLeftMouseButtonUp();
                    break;
                case WM_RBUTTONDOWN:
                    eventHandler.OnRightMouseButtonDown();
                    break;
                case WM_RBUTTONUP:
                    eventHandler.OnRightMouseButtonUp();
                    break;
                case WM_MOUSELEAVE:
                    break;
                case WM_MOUSEWHEEL:
                    break;
                
            }

EndHandler:
            switch (message)
            {
                case WM_CREATE:
                {
                    var userData = (UserData*) ((CREATESTRUCTA*) lParam)->lpCreateParams;
                    _ = SetWindowLongPtrA(hWnd, GWLP_USERDATA, (nint) userData);
                    if (userData != null)
                    {
                        ((IWindowEventHandler) GCHandle.FromIntPtr(userData->EventHandler).Target)?.OnCreate();
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
                var eventHandlerHandle = GCHandle.FromIntPtr(userData->EventHandler);
                eventHandler = eventHandlerHandle.IsAllocated ? (IWindowEventHandler) eventHandlerHandle.Target : null;
                var windowHandle = GCHandle.FromIntPtr(userData->Window);
                window = windowHandle.IsAllocated ? (Win32Window)windowHandle.Target : null;

                Debug.Assert(window is not null && eventHandler is not null, "Handles are unavailable");
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
            UpdateMousePosition();
            return true;
        }

        public void Exit() => PostQuitMessage(0);

        private bool _cursorVisible = true;
        private POINT _hideCursorPosition;
        public unsafe void ToggleMouse()
        {
            _cursorVisible = !_cursorVisible;
            ShowCursor(_cursorVisible);
            if (_cursorVisible)
            {
                SetCursorPos(_hideCursorPosition.X, _hideCursorPosition.Y);
            }
            else
            {
                POINT pos;
                if (GetCursorPos(&pos))
                {
                    _hideCursorPosition = pos;
                }
                SetCursorPos(Width/2, Height/2);
            }
        }

        private unsafe void UpdateMousePosition()
        {
            POINT point;
            if (!GetCursorPos(&point))
            {
                return;
            }

            if (_cursorVisible)
            {
                if (!ScreenToClient(Handle, &point))
                {
                    return;
                }
                if (_mousePosition.X != point.X || _mousePosition.Y != point.Y)
                {
                    _mousePosition = point;
                    _windowEventHandler.OnMouseMove(point);
                }
            }
            else
            {
                var mouseMoved = false;
                var center = new POINT {X = Width / 2, Y = Height / 2};
                var delta = point - center;
                if (delta.X != 0 || delta.Y != 0)
                {
                    _mousePosition += delta; // TODO: this will cause integer overflow at some point
                    mouseMoved = true;
                }
                SetCursorPos(center.X, center.Y);

                if (mouseMoved)
                {
                    _windowEventHandler.OnMouseMove(_mousePosition);
                }
            }
            
        }

        public unsafe void Dispose()
        {
            if (_userData != null)
            {
                GCHandle.FromIntPtr(_userData->EventHandler).Free();
                GCHandle.FromIntPtr(_userData->Window).Free();
                Marshal.FreeHGlobal((nint)_userData);

                DestroyWindow(Handle);
                Handle = 0;
                _userData = null;
            }
        }
    }
}
