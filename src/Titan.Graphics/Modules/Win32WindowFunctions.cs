using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.Win32;
using static Titan.Windows.Win32.User32;
using static Titan.Windows.Win32.WindowsMessage;

namespace Titan.Graphics.Modules;

internal unsafe struct Win32WindowFunctions : IWindowFunctions
{
    private const string _className = nameof(Win32WindowFunctions);

    public static bool CreateWindow(ref nint handle, in WindowDescriptor descriptor, WindowEventQueue* eventQueue)
    {
        var wndClassExA = new WNDCLASSEXA
        {
            CbClsExtra = 0,
            CbSize = (uint)Marshal.SizeOf<WNDCLASSEXA>(),
            //HCursor = _defaultCursor,
            HIcon = 0,
            LpFnWndProc = &WndProc,
            CbWndExtra = 0,
            HIconSm = 0,
            HInstance = Marshal.GetHINSTANCE(typeof(Win32WindowFunctions).Module),
            HbrBackground = 0,
            LpszClassName = _className,
            Style = 0
        };

        Logger.Trace<Win32WindowFunctions>($"RegisterClass {_className}");
        if (RegisterClassExA(wndClassExA) == 0)
        {
            Logger.Error<Win32WindowFunctions>($"{nameof(RegisterClassExA)} failed with Win32Error {Marshal.GetLastWin32Error()}");
            return false;
        }

        // Adjust the window size to take into account for the menu etc
        var wsStyle = WindowStyles.OverlappedWindow | WindowStyles.Visible;
        if (!descriptor.Resizable)
        {
            wsStyle ^= WindowStyles.ThickFrame;
        }
        const int windowOffset = 100;
        var windowRect = new RECT
        {
            Left = windowOffset,
            Top = windowOffset,
            Right = (int)(descriptor.Width + windowOffset),
            Bottom = (int)(descriptor.Height + windowOffset)
        };
        AdjustWindowRect(ref windowRect, wsStyle, false);

        Logger.Trace<Win32WindowFunctions>($"Create window with size Width: {descriptor.Width} Height: {descriptor.Height}");

        handle = CreateWindowExA(
            0,
            _className,
            new string(descriptor.Title), // TODO: replace the "string" parameter with char*/byte*
            wsStyle,
            -1,
            -1,
            windowRect.Right - windowRect.Left,
            windowRect.Bottom - windowRect.Top,
            0,
            0,
            wndClassExA.HInstance,
            eventQueue
        );
        if (handle == 0)
        {
            Logger.Error<Win32WindowFunctions>($"{nameof(CreateWindowExA)} failed with Win32Error {Marshal.GetLastWin32Error()}");
            return false;
        }
        return true;
    }

    public static bool DestroyWindow(ref nint handle)
    {
        var result = true;
        if (handle != 0)
        {
            result = User32.DestroyWindow(handle);
            handle = 0;
        }
        return result;
    }

    public static bool SetTitle(nint handle, ReadOnlySpan<char> title)
    {
        fixed (char* pTitle = title)
        {
            return SetWindowTextW(handle, pTitle) == 0;
        }
    }

    public static bool Show(nint handle) => ShowWindow(handle, ShowWindowCommand.Show) == 0;
    public static bool Hide(nint handle) => ShowWindow(handle, ShowWindowCommand.Hide) == 0;
    public static bool Update(nint handle)
    {
        MSG msg;
        var result = GetMessageA(&msg, 0, 0, 0);

        if (result == -1)
        {
            Logger.Error<Window>($"Windows error occured when trying to get a message of the message queue: {Marshal.GetLastWin32Error()}");
            return true;
        }
        if (msg.Message == WM_INPUT)
        {
            //Logger.Info("VM_INPUT");
        }
        else if (msg.Message == WM_QUIT)
        {
            SetWindowLongPtrA(handle, GWLP_USERDATA, 0);
            return false;
        }

        TranslateMessage(msg);
        DispatchMessage(msg);
        return true;
    }

    public static bool GetRelativeCursorPosition(nint handle, out Point position)
    {
        Unsafe.SkipInit(out position);
        
        POINT point;
        if (GetCursorPos(&point))
        {
            if (ScreenToClient(handle, &point))
            {
                position = new Point(point.X, point.Y);
                return true;
            }
        }
        return false;
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
                Logger.Error<Window>("Failed to read the CreateStruct. Windows events wont be handled.");
            }
            else
            {
                SetWindowLongPtrA(hWnd, GWLP_USERDATA, (nint)pCreateStruct->lpCreateParams);
            }
        }

        var eventQueue = (WindowEventQueue*)GetWindowLongPtrA(hWnd, GWLP_USERDATA);
        if (eventQueue == null)
        {
            Logger.Warning<Win32WindowFunctions>($"Message handler not found, {message} dropped.");
            return DefWindowProcA(hWnd, message, wParam, lParam);
        }

        switch (message)
        {
            case WM_CLOSE:
                PostQuitMessage(0); //NOTE(Jens): This should be handled in some other way, so we can do a proper exit from the engine.
                break;

            case WM_KEYDOWN:
            case WM_SYSKEYDOWN:
                eventQueue->Push(new KeyPressed());
                //WindowEventHandler.OnKeyDown(wParam, lParam);
                break;
            case WM_KEYUP:
            case WM_SYSKEYUP:
                eventQueue->Push(new KeyReleased());
                //WindowEventHandler.OnKeyUp(wParam);
                break;
        }
        return DefWindowProcA(hWnd, message, wParam, lParam);
    }
}

public struct KeyPressed : IWindowEvent
{
    public static uint Id => 14;
}
public struct KeyReleased : IWindowEvent
{
    public static uint Id => 16;
}


public struct WindowLostFocus : IWindowEvent
{
    public static uint Id => 11;
}

public struct WindowGainedFocus : IWindowEvent
{
    public static uint Id => 12;
}
