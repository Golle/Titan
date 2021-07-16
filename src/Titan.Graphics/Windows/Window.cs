using System;
using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Graphics.Windows.Events;
using Titan.Windows;
using Titan.Windows.Win32;
using static Titan.Windows.Win32.User32;
using static Titan.Windows.Win32.WindowsMessage;

namespace Titan.Graphics.Windows
{
    public record WindowConfiguration(string Title, uint Width, uint Height, bool Windowed = true);

    public unsafe class Window : IDisposable
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public bool Windowed { get; }
        internal HWND Handle { get; }

        private string _title;
        private readonly string _className;
        private POINT _center;
        private POINT _mousePosition;
        private POINT _hideCursorPosition;

        private static Window _activeWindow; // TODO: change this if we'll ever support multiple windows
        private bool _cursorVisible = true;
        private static HCURSOR _defaultCursor;

        private Window(HWND handle, string title, string className, uint width, uint height, bool windowed)
        {
            Width =  width;
            Height = height;
            Windowed = windowed;
            Handle = handle;
            _title = title;
            _className = className;
            _activeWindow = this;
        }

        public static Window Create(WindowConfiguration config)
        {
            if (_activeWindow != null)
            {
                throw new NotSupportedException($"Only a single {nameof(Window)} can be created.");
            }

            _defaultCursor = LoadCursorW(0, StandardCursorResources.IDC_HELP);
            var className = "class_" + Guid.NewGuid().ToString().Substring(0, 4);
            // Create the Window Class EX
            var wndClassExA = new  WNDCLASSEXA
            {
                CbClsExtra = 0,
                CbSize = (uint)Marshal.SizeOf<WNDCLASSEXA>(),
                HCursor = _defaultCursor,
                HIcon = 0,
                LpFnWndProc = &WndProc,
                CbWndExtra = 0,
                HIconSm = 0,
                HInstance = Marshal.GetHINSTANCE(typeof(Window).Module),
                HbrBackground = 0,
                LpszClassName = className,
                Style = 0
            };
            Logger.Trace<Window>($"RegisterClass {className}");
            if (RegisterClassExA(wndClassExA) == 0)
            {
                Logger.Error<Window>($"RegisterClassExA failed with Win32Error {Marshal.GetLastWin32Error()}");
                return null;
            }

            // Adjust the window size to take into account for the menu etc
            const WindowStyles wsStyle = WindowStyles.OverlappedWindow | WindowStyles.Visible;
            const int windowOffset = 100;
            var windowRect = new RECT
            {
                Left = windowOffset,
                Top = windowOffset,
                Right = (int) (config.Width + windowOffset),
                Bottom = (int) (config.Height + windowOffset)
            };
            AdjustWindowRect(ref windowRect, wsStyle, false);

            Logger.Trace<Window>($"Create window with size Width: {config.Width} Height: {config.Height}");
            // Create the Window
            var handle = CreateWindowExA(
                0,
                className,
                config.Title,
                wsStyle,
                -1,
                -1,
                windowRect.Right - windowRect.Left,
                windowRect.Bottom - windowRect.Top,
                0,
                0,
                wndClassExA.HInstance,
                null
            );

            
            if (!handle.IsValid)
            {
                Logger.Error($"CreateWindowExA failed with Win32Error {Marshal.GetLastWin32Error()}");
                return null;
            }

            return new Window(handle, config.Title, className, config.Width, config.Height, config.Windowed);
        }


        public void SetTitle(string title) => SetWindowTextA(Handle, title);
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

            foreach (ref readonly var @event in EventManager.GetEvents())
            {
                if (@event.Type == MouseStateEvent.Id)
                {
                    ToggleMouse(@event.As<MouseStateEvent>().Visible);
                }
            }

            return true;
        }

        private void UpdateMousePosition()
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
                    WindowEventHandler.OnMouseMove(point);
                }
            }
            else
            {
                var mouseMoved = false;
                var center = new POINT((int) (Width / 2), (int) (Height / 2));
                var delta = point - center;
                if (delta.X != 0 || delta.Y != 0)
                {
                    _mousePosition += delta; // TODO: this will cause integer overflow at some point
                    mouseMoved = true;
                }
                SetCursorPos(center.X, center.Y);

                if (mouseMoved)
                {
                    WindowEventHandler.OnMouseMove(_mousePosition);
                }
            }
        }

        private void ToggleMouse(bool visible)
        {
            if (visible == _cursorVisible)
            {
                // No state change
                return;
            }

            _cursorVisible = visible;
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
                SetCursorPos((int)(Width / 2), (int)(Height / 2));
            }
        }

        [UnmanagedCallersOnly]
        public static nint WndProc(HWND hWnd, WindowsMessage message, nuint wParam, nuint lParam)
        {
            switch (message)
            {
                case WM_KILLFOCUS:
                    WindowEventHandler.OnLostFocus();
                    break;
                case WM_SETFOCUS:
                    WindowEventHandler.OnSetFocus();
                    break;
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    WindowEventHandler.OnKeyDown(wParam, lParam);
                    break;
                case WM_KEYUP:
                case WM_SYSKEYUP:
                    WindowEventHandler.OnKeyUp(wParam);
                    break;
                case WM_CHAR:
                    WindowEventHandler.OnCharTyped(wParam);
                    break;
                case WM_SIZE:
                    if (_activeWindow != null)
                    {
                        var window = _activeWindow;
                        var width = (uint)(lParam & 0xffff);
                        var height = (uint)((lParam >> 16) & 0xffff);
                        
                        window.Height = height;
                        window.Width = width;
                        window._center = new POINT((int) (window.Width / 2), (int) (window.Height / 2));
                    }
                    else
                    {
                        Logger.Warning<Window>("No active window, changing window size will be ignored.");
                    }
                    break;
                case WM_EXITSIZEMOVE:
                    WindowEventHandler.OnWindowResize(_activeWindow.Width, _activeWindow.Height);
                    break;
                case WM_LBUTTONDOWN:
                    WindowEventHandler.OnLeftMouseButtonDown();
                    break;
                case WM_LBUTTONUP:
                    WindowEventHandler.OnLeftMouseButtonUp();
                    break;
                case WM_RBUTTONDOWN:
                    WindowEventHandler.OnRightMouseButtonDown();
                    break;
                case WM_RBUTTONUP:
                    WindowEventHandler.OnRightMouseButtonUp();
                    break;
                case WM_MOUSELEAVE:
                    break;
                case WM_MOUSEWHEEL:
                    break;
                case WM_CREATE:
                    WindowEventHandler.OnCreate();
                    break;
                case WM_CLOSE:
                    WindowEventHandler.OnClose();
                    PostQuitMessage(0);
                    return 0;
            }
            return DefWindowProcA(hWnd, message, wParam, lParam);
        }

        public void Dispose()
        {
            DestroyWindow(Handle);
            UnregisterClassA(_className, Marshal.GetHINSTANCE(typeof(Window).Module));
            
            _activeWindow = null; // TODO: move this to a windows event?
        }

        public void Show()
        {
            ShowWindow(Handle, ShowWindowCommand.Show);
        }
    }
}
