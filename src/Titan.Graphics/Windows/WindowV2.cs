using System;
using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Graphics.Windows.Input.Raw;
using Titan.Windows;
using Titan.Windows.Win32;
using static Titan.Windows.Win32.User32;
using static Titan.Windows.Win32.WindowsMessage;

namespace Titan.Graphics.Windows;

public sealed unsafe class WindowV2 : IDisposable
{
    private const string _className = nameof(WindowV2);
    private RawInputHandler _rawInputHandler = new();
    public HWND WindowHandle { get; private set; }
    private GCHandle _instanceHandle;

    private WindowV2()
    {
        _instanceHandle = GCHandle.Alloc(this, GCHandleType.Normal);
    }

    public static WindowV2 Create(WindowConfiguration config)
    {
        // Create the Window Class EX
        var wndClassExA = new WNDCLASSEXA
        {
            CbClsExtra = 0,
            CbSize = (uint)Marshal.SizeOf<WNDCLASSEXA>(),
            //HCursor = _defaultCursor,
            HIcon = 0,
            LpFnWndProc = &WndProc,
            CbWndExtra = 0,
            HIconSm = 0,
            HInstance = Marshal.GetHINSTANCE(typeof(WindowV2).Module),
            HbrBackground = 0,
            LpszClassName = _className,
            Style = 0
        };
        Logger.Trace<WindowV2>($"RegisterClass {_className}");
        if (RegisterClassExA(wndClassExA) == 0)
        {
            Logger.Error<WindowV2>($"RegisterClassExA failed with Win32Error {Marshal.GetLastWin32Error()}");
            return null;
        }

        // Adjust the window size to take into account for the menu etc
        var wsStyle = WindowStyles.OverlappedWindow | WindowStyles.Visible;
        if (!config.Resizable)
        {
            wsStyle ^= WindowStyles.ThickFrame;
        }
        const int windowOffset = 100;
        var windowRect = new RECT
        {
            Left = windowOffset,
            Top = windowOffset,
            Right = (int)(config.Width + windowOffset),
            Bottom = (int)(config.Height + windowOffset)
        };
        AdjustWindowRect(ref windowRect, wsStyle, false);

        Logger.Trace<WindowV2>($"Create window with size Width: {config.Width} Height: {config.Height}");

        // Set properties before the window is created so they are available in WM_CREATE
        //Height = config.Height;
        //Width = config.Width;
        //Title = config.Title;

        var window = new WindowV2();

        // Create the Window
        window.WindowHandle = CreateWindowExA(
            0,
            _className,
            config.Title,
            wsStyle,
            -1,
            -1,
            windowRect.Right - windowRect.Left,
            windowRect.Bottom - windowRect.Top,
            0,
            0,
            wndClassExA.HInstance,
            (void*)GCHandle.ToIntPtr(window._instanceHandle)
        );
        window.Show();
        return window;
    }

    public void Show() => ShowWindow(WindowHandle, ShowWindowCommand.Show);
    public void Hide() => ShowWindow(WindowHandle, ShowWindowCommand.Hide);

    public bool Update()
    {
        MSG msg;
        var result = GetMessageA(&msg, WindowHandle, 0, 0);

        if (result == 0)
        {
            SetWindowLongPtrA(WindowHandle, GWLP_USERDATA, 0);
            return false;
        }

        if (result == -1)
        {
            Logger.Error<WindowV2>($"Windows error occured when trying to get a message of the message queue: {Marshal.GetLastWin32Error()}");
            return true;
        }
        if (msg.Message == WM_INPUT)
        {
            //Logger.Info("VM_INPUT");
        }

        TranslateMessage(msg);
        DispatchMessage(msg);
        return true;
    }

    [UnmanagedCallersOnly]
    private static nint WndProc(HWND hWnd, WindowsMessage message, nuint wParam, nuint lParam)
    {
        // Set the USERDATA for this window to the Window instance.
        if (message == WM_CREATE)
        {
            var pCreateStruct = (CREATESTRUCTA*)lParam;
            if (pCreateStruct == null)
            {
                Logger.Error<WindowV2>("Failed to read the CreateStruct. Windows events wont be handled.");
            }
            else
            {
                SetWindowLongPtrA(hWnd, GWLP_USERDATA, (nint)pCreateStruct->lpCreateParams);
            }
        }

        var ptr = GetWindowLongPtrA(hWnd, GWLP_USERDATA);
        if (ptr == 0)
        {
            Logger.Warning($"Message handler not found, {message} dropped.");
            return DefWindowProcA(hWnd, message, wParam, lParam);
        }
        
        var window = (WindowV2)GCHandle.FromIntPtr(ptr).Target;
        // NOTE(Jens): Handle relevant messages here
        switch (message)
        {
            

            case WM_CLOSE:
                PostQuitMessage(0); //NOTE(Jens): This should be handled in some other way, so we can do a proper exit from the engine.
                return 0;

        }
        return DefWindowProcA(hWnd, message, wParam, lParam);
    }

    public void Dispose()
    {
        if (WindowHandle != default)
        {
            DestroyWindow(WindowHandle);
            UnregisterClassA(_className, Marshal.GetHINSTANCE(GetType().Module));
            WindowHandle = default;
        }

        if (_instanceHandle.IsAllocated)
        {
            _instanceHandle.Free();
        }
    }
}
