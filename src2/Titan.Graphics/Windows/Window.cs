using System;
using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.Win32;
using static Titan.Windows.Win32.User32;
using static Titan.Windows.Win32.WindowsMessage;

namespace Titan.Graphics.Windows
{

    public record WindowConfiguration(string Title, uint Width, uint Height, bool Windowed = true);

    public unsafe class Window : IDisposable
    {
        public uint Width { get; }
        public uint Height{ get; }
        public bool Windowed { get; }
        internal HWND Handle { get; }

        private string _title;
        private readonly string _className;
        private Window(HWND handle, string title, string className, uint width, uint height, bool windowed)
        {
            Width = width;
            Height = height;
            Windowed = windowed;
            Handle = handle;
            _title = title;
            _className = className;
        }

        public static Window Create(WindowConfiguration config)
        {
            var className = "class_" + Guid.NewGuid().ToString().Substring(0, 4);
            // Create the Window Class EX
            var wndClassExA = new  WNDCLASSEXA
            {
                CbClsExtra = 0,
                CbSize = (uint)Marshal.SizeOf<WNDCLASSEXA>(),
                HCursor = 0,
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
                null // Add userdata for callback
            );

            if (!handle.IsValid)
            {
                Logger.Error($"CreateWindowExA failed with Win32Error {Marshal.GetLastWin32Error()}");
                return null;
            }

            return new Window(handle, config.Title, className, config.Width, config.Height, config.Windowed);
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
            //UpdateMousePosition();
            return true;
        }


        [UnmanagedCallersOnly]
        public static nint WndProc(HWND hWnd, WindowsMessage message, nuint wParam, nuint lParam)
        {
            switch (message)
            {
                case WM_CREATE:
                {
                    //var userData = (UserData*)((CREATESTRUCTA*)lParam)->lpCreateParams;
                    //_ = SetWindowLongPtrA(hWnd, GWLP_USERDATA, (nint)userData);
                    //if (userData != null)
                    //{
                    //    ((IWindowEventHandler)GCHandle.FromIntPtr(userData->EventHandler).Target)?.OnCreate();
                    //}

                    return 0;
                }
                case WM_CLOSE:
                    //eventHandler?.OnClose();
                    PostQuitMessage(0);
                    return 0;
            }
            return DefWindowProcA(hWnd, message, wParam, lParam);
        }


        public void Dispose()
        {
            DestroyWindow(Handle);
            UnregisterClassA(_className, Marshal.GetHINSTANCE(typeof(Window).Module));
        }

        public void Show()
        {
            ShowWindow(Handle, ShowWindowCommand.Show);
        }
    }
}
