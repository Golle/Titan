using System.Runtime.CompilerServices;

namespace Titan.Windows;

public readonly struct WindowHandle
{
    public readonly nuint Handle;
    public WindowHandle(nuint handle) => Handle = handle;
    public WindowHandle(uint handle) => Handle = handle;
    public WindowHandle(int handle) => Handle = (nuint)handle;
    public WindowHandle(nint handle) => Handle = (nuint)handle;
    public bool IsValid => Handle != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nuint(in WindowHandle handle) => handle.Handle;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator WindowHandle(nuint handle) => new(handle);
    public static implicit operator WindowHandle(uint handle) => new(handle);
    public static implicit operator WindowHandle(nint handle) => new(handle);
    public override string ToString() 
        => Handle.ToString();
}
