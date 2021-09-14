using System;
using System.Diagnostics;
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
    public static unsafe class Window
    {
        private const string ClassName = "titan_game_engine";
        private static HCURSOR _defaultCursor;
        private static POINT _center;
        private static POINT _mousePosition;
        private static POINT _hideCursorPosition;
        private static bool _cursorVisible = true;
        private static Stopwatch _timer;
        private static int _frames;
        public static uint Width { get; private set; }
        public static uint Height { get; private set; }
        public static string Title { get; private set; }

        public static bool Windowed { get; }
        public static HWND Handle { get; private set; }

        public static bool Update()
        {
            _frames++;
            if (_timer.Elapsed.TotalSeconds >= 1.0f)
            {
                var fps = _frames / _timer.Elapsed.TotalSeconds;
                SetTitle($"{Title}. FPS: {(int)fps}");
                _timer.Restart();
                _frames = 1;
            }
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
                else if (@event.Type == ExitEvent.Id)
                {
                    ref readonly var e = ref @event.As<ExitEvent>();
                    Logger.Trace($"Exit event received: {e.ExitCode}", typeof(Window));
                    PostQuitMessage(e.ExitCode);
                }
            }
            return true;

        }
        private static void UpdateMousePosition()
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
                var center = new POINT((int)(Width / 2), (int)(Height / 2));
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

        private static void ToggleMouse(bool visible)
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

        public static bool Init(WindowConfiguration config)
        {
            if (Handle.IsValid)
            {
                throw new InvalidOperationException("The window has already be initialized.");
            }
            _defaultCursor = LoadCursorW(0, StandardCursorResources.IDC_HELP);
            _timer = Stopwatch.StartNew();
            // Create the Window Class EX
            var wndClassExA = new WNDCLASSEXA
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
                LpszClassName = ClassName,
                Style = 0
            };
            Logger.Trace($"RegisterClass {ClassName}", typeof(Window));
            if (RegisterClassExA(wndClassExA) == 0)
            {
                Logger.Error($"RegisterClassExA failed with Win32Error {Marshal.GetLastWin32Error()}", typeof(Window));
                return false;
            }

            // Adjust the window size to take into account for the menu etc
            const WindowStyles wsStyle = WindowStyles.OverlappedWindow | WindowStyles.Visible;
            const int windowOffset = 100;
            var windowRect = new RECT
            {
                Left = windowOffset,
                Top = windowOffset,
                Right = (int)(config.Width + windowOffset),
                Bottom = (int)(config.Height + windowOffset)
            };
            AdjustWindowRect(ref windowRect, wsStyle, false);

            Logger.Trace($"Create window with size Width: {config.Width} Height: {config.Height}", typeof(Window));

            // Set properties before the window is created so they are available in WM_CREATE
            Height = config.Height;
            Width = config.Width;
            Title = config.Title;

            // Create the Window
            Handle = CreateWindowExA(
                0,
                ClassName,
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

            if (!Handle.IsValid)
            {
                Logger.Error($"CreateWindowExA failed with Win32Error {Marshal.GetLastWin32Error()}");
                return false;
            }
            return true;
        }

        public static void SetTitle(string title) => SetWindowTextA(Handle, title);
        public static void Show() => ShowWindow(Handle, ShowWindowCommand.Show);

        public static void Destroy()
        {
            DestroyWindow(Handle);
            UnregisterClassA(ClassName, Marshal.GetHINSTANCE(typeof(Window).Module));

            Handle = default;
        }

        [UnmanagedCallersOnly]
        private static nint WndProc(HWND hWnd, WindowsMessage message, nuint wParam, nuint lParam)
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
                {
                    var width = (uint)(lParam & 0xffff);
                    var height = (uint)((lParam >> 16) & 0xffff);

                    Height = height;
                    Width = width;
                    _center = new POINT((int)(Width / 2), (int)(Height / 2));
                }
                    break;
                case WM_EXITSIZEMOVE:
                    WindowEventHandler.OnWindowResize(Width, Height);
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
                    WindowEventHandler.OnCreate(Width, Height);
                    break;
                case WM_CLOSE:
                    WindowEventHandler.OnClose();
                    PostQuitMessage(0);
                    return 0;
            }

            return DefWindowProcA(hWnd, message, wParam, lParam);
        }
    }
}
