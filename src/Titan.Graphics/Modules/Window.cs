using System;

namespace Titan.Graphics.Modules;

public unsafe struct Window
{
    public nint Handle;
    private readonly WindowFunctions _functions;
    internal Window(WindowFunctions functions)
    {
        _functions = functions;
    }

    public void SetTitle(ReadOnlySpan<char> title) => _functions.SetTitle(Handle, title);
    public bool CreateWindow(WindowDescriptor descriptor) => _functions.CreateWindow(ref Handle, descriptor);
    public bool DestroyWindow() => _functions.DestroyWindow(ref Handle);
    public bool Show() => _functions.Show(Handle);
    public bool Hide() => _functions.Hide(Handle);
    public bool Update() => _functions.Update(Handle);
}

internal unsafe struct WindowFunctions
{
    public delegate*<ref nint, in WindowDescriptor, bool> CreateWindow;
    public delegate*<ref nint, bool> DestroyWindow;
    public delegate*<nint, bool> Show;
    public delegate*<nint, bool> Hide;
    public delegate*<nint, ReadOnlySpan<char>, bool> SetTitle;
    public delegate*<nint, bool> Update;
    public static WindowFunctions Create<T>() where T : IWindowFunctions =>
        new()
        {
            CreateWindow = &T.CreateWindow,
            SetTitle = &T.SetTitle,
            DestroyWindow = &T.DestroyWindow,
            Hide = &T.Hide,
            Show = &T.Show,
            Update = &T.Update
        };
}
