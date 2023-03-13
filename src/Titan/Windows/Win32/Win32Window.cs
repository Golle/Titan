using System.Diagnostics;
using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Core.Maths;
using Titan.Input;
using Titan.Platform.Win32;
using Titan.Platform.Win32.DBT;
using Titan.Windows.Events;
using static Titan.Platform.Win32.Kernel32;
using static Titan.Platform.Win32.USER32;
using static Titan.Platform.Win32.WINDOW_MESSAGE;

namespace Titan.Windows.Win32;

internal unsafe class Win32Window : IWindow
{
    private HWND _handle;
    public WindowHandle Handle => _handle.Value;
    public uint Width { get; set; }
    public uint Height { get; set; }
    public Size Size => new((int)Width, (int)Height);

    public Point GetRelativeCursorPosition()
    {
        POINT point;
        if (GetCursorPos(&point))
        {
            if (ScreenToClient(_handle, &point))
            {
                return new Point(point.X, point.Y);
            }
        }
        Logger.Error<Win32Window>("Failed to get the Cursor position.");
        return default;
    }

    public void SetTitle(string title)
    {
        fixed (char* pTitle = title)
        {
            SetWindowTextW(_handle, pTitle);
        }
    }
    public bool Init(WindowCreationArgs args, WindowEventQueue* eventQueue)
    {
        Debug.Assert(eventQueue != null, "No event queue has been set.");
        if (_handle.IsValid)
        {
            Logger.Error<Win32Window>("The window has already been initialized");
            return false;
        }

        const string className = nameof(Win32Window);

        Logger.Trace<Win32Window>($"Trying to create a {nameof(Win32Window)} with Width: {args.Width} Height: {args.Height} Windowed: {args.Windowed}");
        var moduleHandle = GetModuleHandleW(null);

        fixed (char* pClassName = className)
        {
            var wndClass = new WNDCLASSEXA
            {
                HInstance = moduleHandle,
                CbClsExtra = 0,
                CbSize = (uint)Marshal.SizeOf<WNDCLASSEXA>(),
                //HCursor = _defaultCursor,
                HIcon = 0,
                LpFnWndProc = &WndProc,
                CbWndExtra = 0,
                HIconSm = 0,
                HbrBackground = 0,
                LpszClassName = pClassName,
                Style = 0
            };
            if (RegisterClassExW(&wndClass) == 0)
            {
                Logger.Error<Win32Window>("Failed to register class.");
                return false;
            }
        }

        var wsStyle = WINDOWSTYLES.WS_OVERLAPPEDWINDOW | WINDOWSTYLES.WS_VISIBLE;
        if (!args.Resizable)
        {
            wsStyle ^= WINDOWSTYLES.WS_THICKFRAME;
        }

        WINDOWSTYLES_EX exStyles = 0;
        if (args.AlwaysOnTop)
        {
            exStyles |= WINDOWSTYLES_EX.WS_EX_TOPMOST;
        }
        const int windowOffset = 100;
        var windowRect = new RECT
        {
            Left = windowOffset,
            Top = windowOffset,
            Right = (int)(args.Width + windowOffset),
            Bottom = (int)(args.Height + windowOffset)
        };
        AdjustWindowRect(&windowRect, wsStyle, false);
        Logger.Trace<Win32Window>($"Adjusted window rect. Width: {windowRect.Right - windowRect.Left} Height: {windowRect.Bottom - windowRect.Top}");
        HWND hwnd;
        fixed (char* pTitle = args.Title)
        fixed (char* pClassName = className)
        {
            hwnd = CreateWindowExW(
                exStyles,
                pClassName,
                pTitle,
                wsStyle,
                -1,
                -1,
                windowRect.Right - windowRect.Left,
                windowRect.Bottom - windowRect.Top,
                0,
                0,
                moduleHandle,
                eventQueue // TODO: add callback struct here
            );
        }

        if (!hwnd.IsValid)
        {
            Logger.Error<Win32Window>("Failed to create the window.");
            fixed (char* pClassName = className)
            {
                UnregisterClassW(pClassName, moduleHandle);
            }
            return false;
        }
        Logger.Trace<Win32Window>($"Window created with Handle: {hwnd}");


        _handle = hwnd;
        Height = args.Height;
        Width = args.Width;


        ShowWindow(hwnd, SHOW_WINDOW_COMMANDS.SW_SHOW);
        return true;
    }

    public void Shutdown()
    {
        if (_handle.IsValid)
        {
            DestroyWindow(_handle);
        }
        _handle = 0;
    }

    public bool Update()
    {
        MSG msg;
        while (PeekMessageW(&msg, 0, 0, 0, 1))
        {
            if (msg.Message == WM_QUIT)
            {
                return false;
            }
            //if (msg.Message == WM_KEYDOWN)
            {
                //NOTE(Jens): Add input handling here. it will decrease the input latency by a tiny amount.
            }
            TranslateMessage(&msg);
            DispatchMessageW(&msg);
        }
        return true;
    }

    [UnmanagedCallersOnly]
    private static nint WndProc(HWND hWnd, WINDOW_MESSAGE message, nuint wParam, nuint lParam)
    {
        // Set the USERDATA for this window to the Window instance.
        if (message == WM_CREATE)
        {
            var pCreateStruct = (CREATESTRUCTA*)lParam;
            if (pCreateStruct == null)
            {
                Logger.Error<Win32Window>($"Failed to read the {nameof(CREATESTRUCTA)}. Windows events wont be handled.");
            }
            else
            {
                Logger.Trace<Win32Window>($"Found a create param. Address: 0x{(nint)pCreateStruct->lpCreateParams:x8}");
                SetWindowLongPtrW(hWnd, GWLP_USERDATA, (nint)pCreateStruct->lpCreateParams);
            }

            Logger.Trace<Win32Window>("Register for DeviceNotifications for Audio devices.");
            //NOTE(Jens): Add notifications for Audio device removal/arrival.
            DEV_BROADCAST_DEVICEINTERFACE_W filter = default;
            filter.dbcc_size = (uint)sizeof(DEV_BROADCAST_DEVICEINTERFACE_W);
            filter.dbcc_devicetype = (uint)DBT_DEVICE_TYPES.DBT_DEVTYP_DEVICEINTERFACE;
            filter.dbcc_classguid = DEVICEINTERFACE_AUDIO.DEVINTERFACE_AUDIO_RENDER;

            //NOTE(Jens): We should store this some where so we can release it when the window is closed.
            var notificationHandle = RegisterDeviceNotificationW(hWnd.Value, &filter, DEVICE_NOTIFY_FLAGS.DEVICE_NOTIFY_WINDOW_HANDLE);
            Logger.Warning<Win32Window>($"The handle from {nameof(RegisterDeviceNotificationW)} is not being saved so we can't release it. Fix this.");
            if (notificationHandle.Value == 0)
            {
                Logger.Error<Win32Window>($"The {nameof(RegisterDeviceNotificationW)} failed.");
            }
        }

        var eventQueue = (WindowEventQueue*)GetWindowLongPtrW(hWnd, GWLP_USERDATA);
        if (eventQueue == null)
        {
            Logger.Warning<Win32Window>($"Message handler not found, {message} handled by {nameof(DefWindowProcW)}.");
            return DefWindowProcW(hWnd, message, wParam, lParam);
        }

        switch (message)
        {
            case WM_CLOSE:
                PostQuitMessage(0); //NOTE(Jens): This should be handled in some other way, so we can do a proper exit from the engine.
                break;

            case WM_KEYDOWN:
            case WM_SYSKEYDOWN:
                {
                    var repeat = (lParam & 0x40000000) > 0;
                    var code = (int)wParam;
                    Debug.Assert(code is >= 0 and <= byte.MaxValue);
                    eventQueue->Push(new KeyPressedEvent((KeyCode)code, repeat));
                    break;
                }
            case WM_KEYUP:
            case WM_SYSKEYUP:
                {
                    var code = (int)wParam;
                    Debug.Assert(code is >= 0 and <= byte.MaxValue);
                    eventQueue->Push(new KeyReleasedEvent((KeyCode)code));
                    break;
                }

            case WM_CHAR:
                var character = (char)wParam;
                eventQueue->Push(new CharacterTypedEvent(character));
                break;

            case WM_SIZE:
                var width = (uint)(lParam & 0xffff);
                var height = (uint)((lParam >> 16) & 0xffff);
                eventQueue->Push(new WindowSizeEvent(width, height));
                break;

            case WM_EXITSIZEMOVE:
                eventQueue->Push(new WindowResizeEvent());
                break;

            case WM_KILLFOCUS:
                eventQueue->Push(new WindowLostFocusEvent());
                break;
            case WM_SETFOCUS:
                eventQueue->Push(new WindowGainedFocusEvent());
                break;
            case WM_DEVICECHANGE:
                var devinterface = (DEV_BROADCAST_DEVICEINTERFACE_W*)lParam; // This is also the DEV_BROADCAST_HDR struct
                if (devinterface == null || devinterface->dbcc_devicetype != (nint)DBT_DEVICE_TYPES.DBT_DEVTYP_DEVICEINTERFACE || devinterface->dbcc_classguid != DEVICEINTERFACE_AUDIO.DEVINTERFACE_AUDIO_RENDER)
                {
                    // nothing we care about at the moment
                    break;
                }

                //NOTE(Jens): Add the device name for debugging?
                switch ((DBT_DEVICE_TYPES)wParam)
                {
                    case DBT_DEVICE_TYPES.DBT_DEVICEARRIVAL:
                        eventQueue->Push(new AudioDeviceArrivedEvent());
                        break;
                    case DBT_DEVICE_TYPES.DBT_DEVICEREMOVECOMPLETE:
                        eventQueue->Push(new AudioDeviceRemovedEvent());
                        break;
                }
                break;
        }
        return DefWindowProcW(hWnd, message, wParam, lParam);
    }
}
