namespace Titan.Graphics.Modules;

internal interface IWindowFunctions
{
    static abstract unsafe bool CreateWindow(ref nint handle, in WindowDescriptor descriptor, WindowEventQueue* eventQueue);
    static abstract bool DestroyWindow(ref nint handle);
    static abstract bool SetTitle(nint handle, ReadOnlySpan<char> title);
    static abstract bool Show(nint handle);
    static abstract bool Hide(nint handle);
    static abstract bool Update(nint handle);
    static abstract bool GetRelativeCursorPosition(nint handle, out Point position);
}
