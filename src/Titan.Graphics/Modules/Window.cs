using System;
using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.Graphics.Modules;

public record struct Point(int X, int Y);

public struct Window : IResource
{
    public nint Handle;
    public uint Height;
    public uint Width;
}

public readonly unsafe struct WindowApi : IApi
{ 
    private readonly WindowFunctions _functions;
    internal WindowApi(WindowFunctions functions) => _functions = functions;
    public  bool GetRelativeCursorPosition(in Window window, out Point point) => _functions.GetRelativeCursorPosition(window.Handle, out point);
    public void SetTitle(in Window window, ReadOnlySpan<char> title) => _functions.SetTitle(window.Handle, title);
    public bool CreateWindow(WindowDescriptor descriptor, WindowEventQueue* eventQueue, out Window window)
    {
        Unsafe.SkipInit(out window);
        nint handle = default;
        if (_functions.CreateWindow(ref handle, descriptor, eventQueue))
        {
            window = new Window
            {
                Handle = handle,
                Width = descriptor.Width,
                Height = descriptor.Height
            };
            return true;
        }
        return false;
    }

    public bool DestroyWindow(ref Window window) => _functions.DestroyWindow(ref window.Handle);
    public bool Show(in Window window) => _functions.Show(window.Handle);
    public bool Hide(in Window window) => _functions.Hide(window.Handle);
    public bool Update(in Window window) => _functions.Update(window.Handle);
}
