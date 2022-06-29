using System;

namespace Titan.Graphics.Modules;

public record struct Point(int X, int Y);
public unsafe struct Window
{
    public nint Handle;
    private readonly WindowFunctions _functions;
    private readonly WindowEventQueue* _eventQueue;
    internal Window(WindowFunctions functions, WindowEventQueue* eventQueue)
    {
        _functions = functions;
        _eventQueue = eventQueue;
    }

    public void SetTitle(ReadOnlySpan<char> title) => _functions.SetTitle(Handle, title);
    public bool CreateWindow(WindowDescriptor descriptor) => _functions.CreateWindow(ref Handle, descriptor, _eventQueue);
    public bool DestroyWindow() => _functions.DestroyWindow(ref Handle);
    public bool Show() => _functions.Show(Handle);
    public bool Hide() => _functions.Hide(Handle);
    public bool Update() => _functions.Update(Handle);
    public readonly bool GetRelativeCursorPosition(out Point position) => _functions.GetRelativeCursorPosition(Handle, out position);
}
